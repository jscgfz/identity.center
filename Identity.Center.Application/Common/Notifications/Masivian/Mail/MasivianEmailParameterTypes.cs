using System.Text.Json.Serialization;

namespace Identity.Center.Application.Common.Notifications.Masivian.Mail;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MasivianEmailParameterTypes
{
  [JsonStringEnumMemberName("masiv-template/html")]
  TemplateHtml = 0x1,
  [JsonStringEnumMemberName("text/html")]
  TextHtml = 0x2,
  [JsonStringEnumMemberName("text")]
  Text = 0x3,
}
