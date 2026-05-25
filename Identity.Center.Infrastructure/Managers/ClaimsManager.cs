using System.Security.Claims;
using System.Text.Json;
using Identity.Center.Application.Abstractions.Managers;
using Identity.Center.Application.Common.Caching;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Persistence.Data.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Role = Identity.Center.Domain.Entities.Core.Identity.Role;

namespace Identity.Center.Infrastructure.Managers;

public sealed class ClaimsManager(IServiceProvider provider) : IClaimsManager
{
  private readonly IdentityContext _context = provider.GetRequiredService<IdentityContext>();
  private readonly IDatabase _redis = provider.GetRequiredService<IDatabase>();

  public async Task<IEnumerable<Claim>> ByApkiKey(Guid apiKey, CancellationToken cancellationToken = default)
  {
    string apiKeyKey = RedisKeysCommon.ApiKeyHashKey(apiKey);
    IEnumerable<Claim> claims = [];
    async Task<IEnumerable<Claim>> _factory()
    {
      IEnumerable<Claim> output = await _context.Set<ApiKeyClaim>()
        .Include(x => x.Claim.Group)
        .Include(x => x.Claim.Action)
        .Where(x => x.ApiKeyId == apiKey)
        .Select(x => new Claim(IdentityClaimTypes.Caim, $"{x.Claim.Group.Name}:{x.Claim.Action.Name}"))
        .ToListAsync(cancellationToken);

      if (await _context.Set<ApiKey>().AnyAsync(row => row.Id == apiKey && row.Root, cancellationToken))
        output = output.Append(new(IdentityClaimTypes.Caim, "root"));

      return output;
    }

    if (!await _redis.HashExistsAsync(apiKeyKey, RedisKeysCommon.ClaimsStore))
    {
      claims = await _factory();

      await _redis.HashSetAsync(
        apiKeyKey,
        RedisKeysCommon.ClaimsStore,
        JsonSerializer.Serialize(
          new TemporaryValue<IEnumerable<string>>(
            claims.Select(claim => claim.Value),
            DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(5))
          ),
          JsonSerializerOptions.Web
        )
      );
    }
    else
    {
      string decodedClaims = ((string?)await _redis.HashGetAsync(apiKeyKey, RedisKeysCommon.ClaimsStore))!;
      TemporaryValue<IEnumerable<string>> output = JsonSerializer.Deserialize<TemporaryValue<IEnumerable<string>>>(
        decodedClaims,
        JsonSerializerOptions.Web
      )!;

      if (output.ExpiresAtUtc < DateTimeOffset.UtcNow)
      {
        claims = await _factory();

        await _redis.HashSetAsync(
          apiKeyKey,
          RedisKeysCommon.ClaimsStore,
          JsonSerializer.Serialize(
            new TemporaryValue<IEnumerable<string>>(
              claims.Select(claim => claim.Value),
              DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(5))
            ),
            JsonSerializerOptions.Web
          )
        );
      }
      else
        claims = output.Value.Select(c => new Claim(IdentityClaimTypes.Caim, c));
    }

    return claims;
  }

  public async Task<IEnumerable<Claim>> ByRole(Guid roleId, CancellationToken cancellationToken = default)
  {
    string roleKey = RedisKeysCommon.RoleHashKey(roleId);
    async Task<IEnumerable<Claim>> _factory()
    {
      IEnumerable<Claim> output = await _context.Set<RoleClaim>()
        .Include(x => x.Claim.Group)
        .Include(x => x.Claim.Action)
        .Where(x => x.RoleId == roleId)
        .Select(x => new Claim(IdentityClaimTypes.Caim, $"{x.Claim.Group.Name}:{x.Claim.Action.Name}"))
        .ToListAsync(cancellationToken);

      if (await _context.Set<Role>().AnyAsync(row => row.Id == roleId && row.Root, cancellationToken))
        output = output.Append(new(IdentityClaimTypes.Caim, "root"));

      return output;
    }
    IEnumerable<Claim> claims = [];
    if (!await _redis.HashExistsAsync(roleKey, RedisKeysCommon.ClaimsStore))
    {
      claims = await _factory();

      await _redis.HashSetAsync(
        roleKey,
        RedisKeysCommon.ClaimsStore,
        JsonSerializer.Serialize(
          new TemporaryValue<IEnumerable<string>>(
            claims.Select(claim => claim.Value),
            DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(5))
          ),
          JsonSerializerOptions.Web
        )
      );
    }
    else
    {
      string decodedClaims = ((string?)await _redis.HashGetAsync(roleKey, RedisKeysCommon.ClaimsStore))!;
      TemporaryValue<IEnumerable<string>> output = JsonSerializer.Deserialize<TemporaryValue<IEnumerable<string>>>(
        decodedClaims,
        JsonSerializerOptions.Web
      )!;

      if (output.ExpiresAtUtc < DateTimeOffset.UtcNow)
      {
        claims = await _factory();

        await _redis.HashSetAsync(
          roleKey,
          RedisKeysCommon.ClaimsStore,
          JsonSerializer.Serialize(
            new TemporaryValue<IEnumerable<string>>(
              claims.Select(claim => claim.Value),
              DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(5))
            ),
            JsonSerializerOptions.Web
          )
        );
      }
      else
        claims = output.Value.Select(c => new Claim(IdentityClaimTypes.Caim, c));
    }

    return claims;
  }
}
