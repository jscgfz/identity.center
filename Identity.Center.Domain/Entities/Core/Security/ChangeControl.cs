using System.Text.Json;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Enums;
using Identity.Center.Domain.Primitives;

namespace Identity.Center.Domain.Entities.Core.Security;

public class ChangeControl : Entity<Guid>
{
  public required Guid RoleId { get; set; }
  public ChangeControlStates Status { get; set; }
  public required string Reason { get; set; }
  public required JsonElement AuthorizationDocument { get; set; }
  public required JsonElement CurrentPicture { get; set; }
  public required JsonElement RequestPicture { get; set; }

  public virtual Role Role { get; set; } = default!;
}
