using System.Security.Claims;
using System.Text.Json;
using Identity.Center.Application.Abstractions.Managers;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Persistence.Data.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Identity.Center.Infrastructure.Managers;

public sealed class ClaimsManager(IServiceProvider provider) : IClaimsManager
{
  private readonly IdentityContext _context = provider.GetRequiredService<IdentityContext>();
  private readonly IDatabase _redis = provider.GetRequiredService<IDatabase>();

  public async Task<IEnumerable<Claim>> ByApkiKey(Guid apiKey, CancellationToken cancellationToken = default)
  {
    string apiKeyKey = RedisKeysCommon.ApiKeyHashKey(apiKey);
    IEnumerable<Claim> claims = [];
    if (!await _redis.HashExistsAsync(apiKeyKey, RedisKeysCommon.ClaimsStore))
    {
      claims = await _context.Set<ApiKeyClaim>()
        .Include(x => x.Claim.Group)
        .Include(x => x.Claim.Action)
        .Where(x => x.ApiKeyId == apiKey)
        .Select(x => new Claim(IdentityClaimTypes.Caim, $"{x.Claim.Group.Name}:${x.Claim.Action.Name}"))
        .ToListAsync(cancellationToken);

      await _redis.HashSetAsync(
        apiKeyKey,
        RedisKeysCommon.ClaimsStore,
        JsonSerializer.Serialize(
          claims.Select(claim => claim.Value),
          JsonSerializerOptions.Web
        )
      );
    }
    else
    {
      string decodedClaims = ((string?)await _redis.HashGetAsync(apiKeyKey, RedisKeysCommon.ClaimsStore))!;
      claims = JsonSerializer.Deserialize<IEnumerable<string>>(
        decodedClaims,
        JsonSerializerOptions.Web
      )!
        .Select(row => new Claim(IdentityClaimTypes.Caim, row));
    }

    return claims;
  }

  public async Task<IEnumerable<Claim>> ByRole(Guid roleId, CancellationToken cancellationToken = default)
  {
    string roleKey = RedisKeysCommon.RoleHashKey(roleId);
    IEnumerable<Claim> claims = [];
    if (!await _redis.HashExistsAsync(roleKey, RedisKeysCommon.ClaimsStore))
    {
      claims = await _context.Set<RoleClaim>()
        .Include(x => x.Claim.Group)
        .Include(x => x.Claim.Action)
        .Where(x => x.RoleId == roleId)
        .Select(x => new Claim(IdentityClaimTypes.Caim, $"{x.Claim.Group.Name}:${x.Claim.Action.Name}"))
        .ToListAsync(cancellationToken);

      await _redis.HashSetAsync(
        roleKey,
        RedisKeysCommon.ClaimsStore,
        JsonSerializer.Serialize(
          claims.Select(claim => claim.Value),
          JsonSerializerOptions.Web
        )
      );
    }
    else
    {
      string desocedClaims = ((string?)await _redis.HashGetAsync(roleKey, RedisKeysCommon.ClaimsStore))!;
      claims = JsonSerializer.Deserialize<IEnumerable<string>>(
        desocedClaims,
        JsonSerializerOptions.Web
      )!
        .Select(row => new Claim(IdentityClaimTypes.Caim, row));
    }

    return claims;
  }
}
