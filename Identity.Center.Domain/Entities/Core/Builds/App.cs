using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Domain.Primitives;

namespace Identity.Center.Domain.Entities.Core.Builds;

public class App : MasterEntity<Guid>
{
  public long Index { get; set; }
  public required string Prefix { get; set; }
  public string? DomainName { get; set; }

  public virtual AppAuth Auth { get; set; } = default!;
  public virtual ICollection<ApiKey> ApiKeys { get; set; } = [];
  public virtual ICollection<AppAllowedCredential> AllowedCredentials { get; set; } = [];
  public virtual ICollection<AppConfigurationSection> ConfigurationSections { get; set; } = [];
  public virtual ICollection<SingleCredential> Credentials { get; set; } = [];
  public virtual ICollection<Role> Roles { get; set; } = [];
  public virtual ICollection<HealtCheck> HealtChecks { get; set; } = [];
}
