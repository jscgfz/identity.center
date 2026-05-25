using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Identity.Center.Application.Abstractions.Managers;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Common.Caching;
using Identity.Center.Application.Features.Authentication.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Role = Identity.Center.Domain.Entities.Core.Identity.Role;

namespace Identity.Center.Infrastructure.Managers;

public sealed class TokenManager(IServiceProvider provider) : ITokenManager
{
  internal sealed record SessionAtomicValues(
    Guid SessionId,
    string RefreshToken
  );

  private readonly IIdentityRepository<User> _userRepo = provider.GetRequiredService<IIdentityRepository<User>>();
  private readonly IIdentityRepository<Role> _roleRepo = provider.GetRequiredService<IIdentityRepository<Role>>();
  private readonly IIdentityRepository<AppAuth> _appAuthRepo = provider.GetRequiredService<IIdentityRepository<AppAuth>>();
  private readonly IHttpContextAccessor _context = provider.GetRequiredService<IHttpContextAccessor>();
  private readonly IDatabase _redis = provider.GetRequiredService<IDatabase>();

  public async Task<Result<Unit>> ValidateSession()
  {
    if (
      _context.HttpContext == null ||
      !_context.HttpContext.Request.Cookies.TryGetValue("accesToken", out string? accesToken)
    )
      return Result.Failure<Unit>(
        HttpStatusCode.Forbidden,
        new BaseError("Params.UnCompleted", "No autorizado (datos de session errones)")
      );

    JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
    JwtSecurityToken token = jwtSecurityTokenHandler.ReadJwtToken(accesToken);

    if (
      !Guid.TryParse(
          token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
          out Guid userId) ||
      !Guid.TryParse(
          token.Claims.FirstOrDefault(c => c.Type == IdentityClaimTypes.App)?.Value ?? string.Empty,
          out Guid appId) ||
      token.Claims.FirstOrDefault(c => c.Type == "sid")?.Value is not string sid
    )
      return Result.Failure<Unit>(
        HttpStatusCode.Forbidden,
        new BaseError("Session.Expired", "Sesión expirada")
      );

    TemporaryValue<SessionAtomicValues> sessionValues = JsonSerializer.Deserialize<TemporaryValue<SessionAtomicValues>>(
      (string?)await _redis.HashGetAsync(
        RedisKeysCommon.SessionHashKey(userId),
        appId.ToString()
      ) ?? string.Empty
    )!;

    if (sessionValues.ExpiresAtUtc < DateTime.UtcNow)
      return Result.Failure<Unit>(
        HttpStatusCode.Forbidden,
        new BaseError("Session.Expired", "Sesión expirada")
      );

    if (sessionValues.Value.SessionId != Guid.Parse(sid))
    {
      _context.HttpContext.Items.Add($"{nameof(Result)}:Session.Expired", "Se ha iniciado sesión desde otro dispositivo");
      return Result.Failure<Unit>(
        HttpStatusCode.Forbidden,
        new BaseError("Session.Expired", "Se ha iniciado sesión desde otro dispositivo")
      );
    }

    return Unit.Value;
  }

  public async Task<Result<AuthenticationReponseDto>> FromUser(Guid userId, Guid appId, IEnumerable<string>? domainRoles = null, MfaStates? mfaOverride = null, CancellationToken cancellationToken = default)
  {
    User user = await _userRepo.Data.FirstAsync(row => row.Id == userId, cancellationToken);

    string? domainCache = await _redis.HashGetAsync(
      RedisKeysCommon.SessionHashKey(userId),
      nameof(domainRoles)
    );

    if (domainCache != null)
    {
      TemporaryValue<IEnumerable<string>> values = JsonSerializer.Deserialize<TemporaryValue<IEnumerable<string>>>(
        domainCache
      )!;

      if (values.ExpiresAtUtc >= DateTimeOffset.UtcNow)
        domainRoles = values.Value;

      await _redis.HashDeleteAsync(
        RedisKeysCommon.SessionHashKey(userId),
        nameof(domainRoles)
      );
    }

    IEnumerable<Role> roles = await _roleRepo.Data
      .AsNoTracking()
      .Include(row => row.App)
      .Where(row => row.AppId == appId)
      .Where(row =>
        (!row.ActiveDirectoryMandatory && row.Users.Any(u => u.UserId == userId)) ||
        (domainRoles != null && !string.IsNullOrEmpty(row.DomainName) && domainRoles.Contains(row.DomainName))
      )
      .ToListAsync(cancellationToken);

    if (!roles.Any())
      return Result.Failure<AuthenticationReponseDto>(
        HttpStatusCode.Forbidden,
        new BaseError("Role.NotParametrized", "No hay roles parametrizados para el usuario")
      );

    AppAuth? auth = await _appAuthRepo.Data
      .AsNoTracking()
      .Include(row => row.App)
      .FirstOrDefaultAsync(row => row.AppId == appId, cancellationToken);

    if (auth == null)
      return Result.Failure<AuthenticationReponseDto>(
        HttpStatusCode.Conflict,
        new BaseError("App.NotParametrized", "No se ha parametrizado la seguridad de la aplicación")
      );

    Guid sessionId = Guid.NewGuid();

    IEnumerable<Claim> claims = [
      new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
      new Claim(IdentityClaimTypes.App, appId.ToString()),
      ..roles.Select(r => new Claim(ClaimTypes.Role, r.Id.ToString())),
      new Claim(IdentityClaimTypes.Mfa, IdentityCommons.Serialize(mfaOverride ?? (auth.TwoFactorEnabled ? MfaStates.Pending : MfaStates.NotRequired))),
      new Claim("sid", sessionId.ToString())
    ];

    SymmetricSecurityKey symmetricSecurity = new(auth.SignatureKey);
    SigningCredentials signingCredentials = new(symmetricSecurity, SecurityAlgorithms.HmacSha256Signature);
    DateTimeOffset expires = DateTimeOffset.UtcNow.Add(auth.ExpirationTime);
    JwtSecurityToken jwtSecurityToken = new(
      auth.App.Prefix,
      _context.HttpContext?.Request.Host.Host ?? "unknown",
      claims,
      DateTimeOffset.UtcNow.DateTime,
      expires.DateTime,
      signingCredentials
    );

    DateTimeOffset refreshExpire = DateTimeOffset.UtcNow.Add(auth.RefreshTime);

    JwtSecurityTokenHandler jwtSecurityTokenHandler = new();

    string token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
    string refreshToken = Convert.ToBase64String(IdentityCommons.NewHashKey);

    if (domainRoles != null && domainRoles.Any())
      await _redis.HashSetAsync(
        RedisKeysCommon.SessionHashKey(user.Id),
        nameof(domainRoles),
        JsonSerializer.Serialize(
          new TemporaryValue<IEnumerable<string>>(
            domainRoles,
            refreshExpire
          )
        )
      );

    await _redis.HashSetAsync(
      RedisKeysCommon.SessionHashKey(user.Id),
      appId.ToString(),
      JsonSerializer.Serialize(
        new TemporaryValue<SessionAtomicValues>(
          new SessionAtomicValues(sessionId, refreshToken),
          refreshExpire
        )
      )
    );

    _context.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions()
    {
      Expires = refreshExpire,
      HttpOnly = true
    });

    _context.HttpContext?.Response.Cookies.Append("accesToken", token, new CookieOptions()
    {
      Expires = refreshExpire,
      HttpOnly = true
    });

    return new AuthenticationReponseDto(
      user.Id,
      user.FullName,
      DateTimeOffset.UtcNow,
      expires,
      roles.Select(r => new RoleAuthResponseDto(
        r.Id,
        r.Name,
        new Application.Common.Response.MasterOption<Guid>(
          r.App.Id,
          r.App.Name
        )
      )),
      mfaOverride ?? (auth.TwoFactorEnabled ? MfaStates.Pending : MfaStates.NotRequired)
    );
  }

  public async Task<Result<string>> RefreshToken(CancellationToken cancellationToken)
  {
    if (
      _context.HttpContext == null ||
      !_context.HttpContext.Request.Cookies.TryGetValue("accesToken", out string? accesToken) ||
      !_context.HttpContext.Request.Cookies.TryGetValue("refreshToken", out string? refreshToken)
    )
      return Result.Failure<string>(
        HttpStatusCode.Forbidden,
        new BaseError("Params.UnCompleted", "No autorizado (datos de session errones)")
      );

    JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
    JwtSecurityToken token = jwtSecurityTokenHandler.ReadJwtToken(accesToken);

    if (
      token.Claims.FirstOrDefault(c => c.Type == "sid")?.Value is not string sid
    )
      return Result.Failure<string>(
        HttpStatusCode.Forbidden,
        new BaseError("Params.UnCompleted", "No autorizado (datos de session errones)")
      );

    if (
      !token.Claims.Any(c => c.Type == IdentityClaimTypes.App) ||
      !token.Claims.Any(c => c.Type == ClaimTypes.NameIdentifier)
    )
      return Result.Failure<string>(
        HttpStatusCode.Forbidden,
        new BaseError("Params.UnCompleted", "No autorizado (datos de session errones)")
      );

    Guid appId = Guid.Parse(
      token.Claims.First(c => c.Type == IdentityClaimTypes.App).Value
    );
    Guid userId = Guid.Parse(
      token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
    );

    TemporaryValue<SessionAtomicValues> sessionValues = JsonSerializer.Deserialize<TemporaryValue<SessionAtomicValues>>(
      (string?)await _redis.HashGetAsync(
        RedisKeysCommon.SessionHashKey(userId),
        appId.ToString()
      ) ?? string.Empty
    )!;

    if (sessionValues.ExpiresAtUtc < DateTime.UtcNow)
      return Result.Failure<string>(
        HttpStatusCode.Forbidden,
        new BaseError("Session.Expired", "Sesión expirada")
      );

    if (sessionValues.Value.SessionId != Guid.Parse(sid) || sessionValues.Value.RefreshToken != refreshToken)
      return Result.Failure<string>(
        HttpStatusCode.Forbidden,
        new BaseError("Session.Expired", "Se ha iniciado sesión desde otro dispositivo")
      );

    AppAuth auth = await _appAuthRepo.Data
      .AsNoTracking()
      .Include(row => row.App)
      .FirstAsync(row => row.AppId == appId, cancellationToken);

    SymmetricSecurityKey symmetricSecurity = new(auth.SignatureKey);
    SigningCredentials signingCredentials = new(symmetricSecurity, SecurityAlgorithms.HmacSha256Signature);
    DateTimeOffset expires = DateTimeOffset.UtcNow.Add(auth.ExpirationTime);
    JwtSecurityToken jwtSecurityToken = new(
      auth.App.Prefix,
      _context.HttpContext?.Request.Host.Host ?? "unknown",
      token.Claims,
      DateTimeOffset.UtcNow.DateTime,
      expires.DateTime,
      signingCredentials
    );

    DateTimeOffset refreshExpire = DateTimeOffset.UtcNow.Add(auth.RefreshTime);

    string outputToken = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
    refreshToken = Convert.ToBase64String(IdentityCommons.NewHashKey);

    await _redis.HashSetAsync(
      RedisKeysCommon.SessionHashKey(userId),
      appId.ToString(),
      JsonSerializer.Serialize(
        new TemporaryValue<SessionAtomicValues>(
          new SessionAtomicValues(Guid.Parse(sid), refreshToken),
          refreshExpire
        )
      )
    );

    _context.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions()
    {
      Expires = refreshExpire,
      HttpOnly = true
    });

    _context.HttpContext?.Response.Cookies.Append("accesToken", outputToken, new CookieOptions()
    {
      Expires = refreshExpire,
      HttpOnly = true
    });

    return outputToken;
  }
}