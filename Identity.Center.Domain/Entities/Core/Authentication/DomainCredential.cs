using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Primitives;

namespace Identity.Center.Domain.Entities.Core.Authentication;

public class DomainCredential : Entity<Guid>
{
  public required Guid UserId { get; set; }
  public required int CredentialTypeId { get; set; }
  public required string Username { get; set; }

  public virtual User User { get; set; } = default!;
  public virtual CredentialType CredentialType { get; set; } = default!;
}
