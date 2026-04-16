using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Primitives.Abstractions;

namespace Identity.Center.Domain.Entities.Core.Authorization;

public class ApiKeyClaim : IAuditEntityFields<Guid>
{
  public required Guid ApiKeyId { get; set; }
  public required Guid ClaimId { get; set; }
  public DateTimeOffset CreatedAtUtc { get; set; }
  public Guid CreatedBy { get; set; }
  public DateTimeOffset? LastModifiedAtUtc { get; set; }
  public Guid? LastModifiedBy { get; set; }
  public bool IsDeleted { get; set; }
  public DateTimeOffset? DeletedAtUtc { get; set; }
  public Guid? DeletedBy { get; set; }

  public virtual ApiKey ApiKey { get; set; } = default!;
  public virtual ClaimValue Claim { get; set; } = default!;
}
