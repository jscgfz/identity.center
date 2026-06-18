using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Domain.Primitives;

namespace Identity.Center.Domain.Entities.Core.Identity;

public class Role : Entity<Guid>
{
  public required Guid AppId { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  public string? DomainName { get; set; }
  public bool ActiveDirectoryMandatory { get; set; }
  public bool Root { get; set; }
  public virtual App App { get; set; } = default!;
  public virtual ICollection<UserRole> Users { get; set; } = [];
  public virtual ICollection<RoleClaim> Claims { get; set; } = [];
  public virtual ICollection<ChangeControl> History { get; set; } = [];
}
