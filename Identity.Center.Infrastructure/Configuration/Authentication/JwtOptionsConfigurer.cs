using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Identity.Center.Application.Abstractions.Managers;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Persistence.Data.Core;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Center.Infrastructure.Configuration.Authentication;

internal class JwtOptionsConfigurer(IServiceProvider provider) : IConfigureNamedOptions<JwtBearerOptions>
{
  private readonly IdentityContext _context = provider.GetRequiredService<IDbContextFactory<IdentityContext>>().CreateDbContext();

  public void Configure(string? name, JwtBearerOptions options)
  {
    options.TokenValidationParameters = new()
    {
      ValidateLifetime = true,
      ValidateIssuer = true,
      ClockSkew = TimeSpan.Zero,
      LogValidationExceptions = true,
      RequireAudience = false,
      RequireExpirationTime = true,
      RequireSignedTokens = true,
      ValidAlgorithms = [SecurityAlgorithms.HmacSha256Signature],
      IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) => IssuerSigningKeyResolver(token, securityToken, kid, validationParameters, _context),
      ValidIssuers = _context.Set<App>().Select(row => row.Prefix).AsEnumerable(),
      AudienceValidator = (audiences, securityToken, validationParameters) => true
    };
    options.Events = new()
    {
      OnMessageReceived = context =>
      {
        if(context.Request.Query.TryGetValue("access_token", out StringValues accesToken))
          context.Token = accesToken.Single();
        if (context.Request.Cookies.TryGetValue("accesToken", out string? cookieToken))
          context.Token = cookieToken;
        return Task.CompletedTask;
      },
      OnAuthenticationFailed = async context =>
      {
        if(context.Exception is SecurityTokenExpiredException)
        {
          Result<string> refresh = await context.HttpContext.RequestServices.GetRequiredService<ITokenManager>()
            .RefreshToken(context.HttpContext.RequestAborted);

          if (refresh.Success)
          {
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(refresh.Value, context.Options.TokenValidationParameters, out _);
            context.Principal = principal;
            context.Success();
          }
          else
            context.Fail(refresh.Errors.First().Seralize().Value?.ToString() ?? "Unknown reasons");
        }
      },
      OnTokenValidated = async context =>
      {
        Result<Unit> sessionValidation = await context.HttpContext.RequestServices.GetRequiredService<ITokenManager>()
          .ValidateSession();

        if (sessionValidation.Failed)
          context.Fail(string.Join(", ", sessionValidation.Errors.Select(e => e.Seralize().Value)));
      }
    };
  }

  public void Configure(JwtBearerOptions options)
    => Configure(JwtBearerDefaults.AuthenticationScheme, options);

  private static IEnumerable<SecurityKey> IssuerSigningKeyResolver(string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters, IdentityContext context)
  {
    JwtSecurityTokenHandler handler = new();
    JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(token);
    return jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == IdentityClaimTypes.App) is not Claim appIdClaim ||
      !Guid.TryParse(appIdClaim.Value, out Guid appId) ? [] :
      context.Set<AppAuth>().Where(row => row.AppId == appId).Select(row => new SymmetricSecurityKey(row.SignatureKey)).AsEnumerable();
  }
}
