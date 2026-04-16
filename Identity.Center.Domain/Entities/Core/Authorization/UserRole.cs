using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Primitives.Abstractions;

namespace Identity.Center.Domain.Entities.Core.Authorization;

public class UserRole : IAuditEntityFields<Guid>
{
  public required Guid UserId { get; set; }
  public required Guid RoleId { get; set; }
  public DateTimeOffset CreatedAtUtc { get; set; }
  public Guid CreatedBy { get; set; }
  public DateTimeOffset? LastModifiedAtUtc { get; set; }
  public Guid? LastModifiedBy { get; set; }
  public bool IsDeleted { get; set; }
  public DateTimeOffset? DeletedAtUtc { get; set; }
  public Guid? DeletedBy { get; set; }

  public virtual User User { get; set; } = default!;
  public virtual Role Role { get; set; } = default!;
}
