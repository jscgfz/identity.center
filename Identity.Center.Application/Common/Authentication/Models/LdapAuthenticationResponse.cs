using System.Text.Json.Serialization;

namespace Identity.Center.Application.Common.Authentication.Models;

public sealed record LdapAuthenticationResponse(
  [property: JsonPropertyName("mensaje")]  LdapAuthenticationMessage Message,
  IEnumerable<LdapAuthenticationRole> Roles
);

public sealed record LdapAuthenticationMessage(
  [property: JsonPropertyName("codigoMensaje")] int Code,
  [property: JsonPropertyName("descMensaje")] string Description
);

public sealed record LdapAuthenticationRole(
  [property: JsonPropertyName("descripcion")] string Description
);
