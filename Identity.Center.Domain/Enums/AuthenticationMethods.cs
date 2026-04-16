using System.Text.Json.Serialization;

namespace Identity.Center.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AuthenticationMethods
{
  [JsonStringEnumMemberName("user-password")] Single,
  [JsonStringEnumMemberName("qd-endpoint")] Quamtum,
  [JsonStringEnumMemberName("ldap")] LDAP,
}
