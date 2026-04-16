using System.Text.Json;
using Identity.Center.Domain.Enums;
using Identity.Center.Domain.Primitives;

namespace Identity.Center.Domain.Entities.Core.Builds;

public class HealtCheck : Entity<Guid>
{
  public required Guid AppId { get; set; }
  public required string Name { get; set; }
  public required HealtCheckTypes HealtCheckType { get; set; }
  public JsonElement Arguments { get; set; }

  public virtual App App { get; set; } = default!;
}
