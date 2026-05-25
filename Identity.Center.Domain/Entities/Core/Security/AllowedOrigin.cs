using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Primitives;

namespace Identity.Center.Domain.Entities.Core.Security;

public class AllowedOrigin : Entity<Guid>
{
  public required Guid ApiKeyId { get; set; }
  public required string Origin { get; set; }

  public virtual ApiKey ApiKey { get; set; } = default!;
}
