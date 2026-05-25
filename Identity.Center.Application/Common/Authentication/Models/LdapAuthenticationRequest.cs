using System.Text.Json.Serialization;

namespace Identity.Center.Application.Common.Authentication.Models;

public record LdapAuthenticationRequest(
  [property: JsonPropertyName("usuario")] string UserName,
  [property: JsonPropertyName("clave")] string Password,
  [property: JsonPropertyName("dominio")] string? DomainName,
  [property: JsonPropertyName("firma")] string? Key
);
