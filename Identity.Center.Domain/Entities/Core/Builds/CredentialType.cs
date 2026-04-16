using System.Text.Json;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Enums;
using Identity.Center.Domain.Primitives;

namespace Identity.Center.Domain.Entities.Core.Builds;

public class CredentialType : MasterEntity<int>
{
  public required AuthenticationMethods AuthType { get; set; }
  public required JsonElement Arguments { get; set; }

  public virtual ICollection<AppAllowedCredential> Apps { get; set; } = [];
  public virtual ICollection<DomainCredential> DomainCredentials { get; set; } = [];
}
