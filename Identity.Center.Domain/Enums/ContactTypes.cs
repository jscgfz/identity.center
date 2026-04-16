using System.Text.Json.Serialization;

namespace Identity.Center.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContactTypes
{
  [JsonStringEnumMemberName("coorp-mail")] CorporativeMail,
  [JsonStringEnumMemberName("external-mail")] ExternalMail,
  [JsonStringEnumMemberName("cellphone")] Cellphone
}
