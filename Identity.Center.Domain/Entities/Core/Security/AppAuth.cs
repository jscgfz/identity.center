using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Primitives.Abstractions;

namespace Identity.Center.Domain.Entities.Core.Security;

public class AppAuth : IAuditEntityFields<Guid>
{
  public required Guid AppId { get; set; }
  public required byte[] SignatureKey { get; set; }
  public bool TwoFactorEnabled { get; set; }
  public TimeSpan ExpirationTime { get; set; }
  public TimeSpan RefreshTime { get; set; }
  public DateTimeOffset CreatedAtUtc { get; set; }
  public Guid CreatedBy { get; set; }
  public DateTimeOffset? LastModifiedAtUtc { get; set; }
  public Guid? LastModifiedBy { get; set; }
  public bool IsDeleted { get; set; }
  public DateTimeOffset? DeletedAtUtc { get; set; }
  public Guid? DeletedBy { get; set; }

  public virtual App App { get; set; } = default!;
}
