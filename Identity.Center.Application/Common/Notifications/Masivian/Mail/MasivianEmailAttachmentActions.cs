using System.Text.Json.Serialization;

namespace Identity.Center.Application.Common.Notifications.Masivian.Mail;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MasivianEmailAttachmentActions
{
  [JsonStringEnumMemberName("generate-pdf")]
  GeneratePdf = 0x1,
  [JsonStringEnumMemberName("download-delete")]
  DownloadDelete = 0x2,
}
