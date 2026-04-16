using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Domain.Primitives;

namespace Identity.Center.Domain.Entities.Core.Security;

public class Action : MasterEntity<Guid>
{
  public virtual ICollection<ClaimValue> Claims { get; set; } = [];
}
