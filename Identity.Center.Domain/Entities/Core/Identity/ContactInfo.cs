using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Domain.Primitives;

namespace Identity.Center.Domain.Entities.Core.Identity;

public class ContactInfo : Entity<Guid>
{
  public required Guid UserId { get; set; }
  public required Guid ContactTypeId { get; set; }
  public required string Value { get; set; }
  public required byte[] Salt { get; set; }
  public bool Confirmed { get; set; }

  public virtual User User { get; set; } = default!;
  public virtual ContactType ContactType { get; set; } = default!;
}
