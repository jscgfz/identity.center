using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Domain.Primitives;

namespace Identity.Center.Domain.Entities.Core.Authentication;

public class ApiKey : MasterEntity<Guid>
{
  public required Guid AppId { get; set; }
  public required byte[] Hash { get; set; }
  public required byte[] Salt { get; set; }
  public bool Root { get; set; }

  public virtual App App { get; set; } = default!;
  public virtual ICollection<ApiKeyClaim> Claims { get; set; } = [];
  public virtual ICollection<AllowedOrigin> AllowedOrigins { get; set; } = [];
}
