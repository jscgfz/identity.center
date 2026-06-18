using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Domain.Primitives;
using Action = Identity.Center.Domain.Entities.Core.Security.Action;

namespace Identity.Center.Domain.Entities.Core.Authorization;

public class ClaimValue : Entity<Guid>
{
  public required Guid ActionId { get; set; }
  public required Guid GroupId { get; set; }

  public virtual Action Action { get; set; } = default!;
  public virtual Group Group { get; set; } = default!;
  public virtual ICollection<RoleClaim> Roles { get; set; } = [];
  public virtual ICollection<ApiKeyClaim> ApiKeys { get; set; } = [];
  public virtual ICollection<RouteClaim> Routes { get; set; } = [];
}
