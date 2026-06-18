using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Domain.Primitives;

namespace Identity.Center.Domain.Entities.Core.Builds;

public class AppRoute : Entity<Guid>
{
  public required Guid AppId { get; set; }
  public required string Key { get; set; }
  public required string Name { get; set; }
  public required string Path { get; set; }
  public bool ExcludeNav { get; set; }
  public int Index { get; set; }
  public string? Icon { get; set; }
  public Guid? ParentRouteId { get; set; }

  public virtual App App { get; set; } = default!;
  public virtual AppRoute ParentRoute { get; set; } = default!;
  public virtual ICollection<AppRoute> ChildRoutes { get; set; } = [];
  public virtual ICollection<RouteClaim> Claims { get; set; } = [];
}
