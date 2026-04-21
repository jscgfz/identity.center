using System.Security.Claims;

namespace Identity.Center.Application.Abstractions.Managers;

public interface IClaimsManager
{
  Task<IEnumerable<Claim>> ByRole(Guid roleId, CancellationToken cancellationToken = default);
  Task<IEnumerable<Claim>> ByApkiKey(Guid apiKey, CancellationToken cancellationToken = default);
}
