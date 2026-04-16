using Identity.Center.Domain.Primitives.Abstractions;

namespace Identity.Center.Domain.Entities.Core.Builds;

public class AppAllowedCredential : IAuditEntityFields<Guid>
{
  public required Guid AppId { get; set; }
  public required int CredentialTypeId { get; set; }
  public DateTimeOffset CreatedAtUtc { get; set; }
  public Guid CreatedBy { get; set; }
  public DateTimeOffset? LastModifiedAtUtc { get; set; }
  public Guid? LastModifiedBy { get; set; }
  public bool IsDeleted { get; set; }
  public DateTimeOffset? DeletedAtUtc { get; set; }
  public Guid? DeletedBy { get; set; }

  public virtual App App { get; set; } = default!;
  public virtual CredentialType CredentialType { get; set; } = default!;
}
