using System.Security.Claims;
using Identity.Center.Application.Abstractions.Managers;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Persistence.Data.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Infrastructure.Configuration.Authorization;

internal sealed class DbClaimsInjectionTransformation(IServiceProvider provider) : IClaimsTransformation
{
  private readonly IClaimsManager _claimsManager = provider.GetRequiredService<IClaimsManager>();
  private readonly IdentityContext _context = provider.GetRequiredService<IdentityContext>();

  public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
  {
    IEnumerable<Claim> claims = [];
    if (principal.FindFirstValue(ClaimTypes.NameIdentifier) is string name && Guid.TryParse(name, out Guid identifier))
      if (await _context.Set<User>().AnyAsync(row => row.Id == identifier))
      {
        if (
          principal.FindFirstValue(ClaimTypes.Role) is not string role ||
          !Guid.TryParse(role, out Guid roleGuid)
        )
          return principal;

        claims = await _claimsManager.ByRole(identifier);
      }
      else if (await _context.Set<ApiKey>().AnyAsync(row => row.Id == identifier))
        claims = await _claimsManager.ByApkiKey(identifier);

    ClaimsIdentity identity = new(claims);
    principal.AddIdentity(identity);
    return principal;
  }
}
