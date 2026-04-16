using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Primitives.Abstractions;

namespace Identity.Center.Domain.Entities.Core.Authentication;

public class SingleCredential : IAuditEntityFields<Guid>
{
  public required Guid UserId { get; set; }
  public required Guid AppId { get; set; }
  public required string Username { get; set; }
  public required byte[] Hash { get; set; }
  public required byte[] Salt { get; set; }
  public DateTimeOffset CreatedAtUtc { get; set; }
  public Guid CreatedBy { get; set; }
  public DateTimeOffset? LastModifiedAtUtc { get; set; }
  public Guid? LastModifiedBy { get; set; }
  public bool IsDeleted { get; set; }
  public DateTimeOffset? DeletedAtUtc { get; set; }
  public Guid? DeletedBy { get; set; }

  public virtual User User { get; set; } = default!;
  public virtual App App { get; set; } = default!;
}
