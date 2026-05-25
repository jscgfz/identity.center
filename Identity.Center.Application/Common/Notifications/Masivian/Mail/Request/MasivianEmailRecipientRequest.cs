namespace Identity.Center.Application.Common.Notifications.Masivian.Mail.Request;

public sealed record MasivianEmailRecipientRequest(
  string To,
  string? Cellphone = null,
  string? From = null,
  string? Subject = null,
  IEnumerable<MasivianEmailParameterRequest>? Parameters = null,
  IEnumerable<MasivianEmailAttachmentRequest>? Attachments = null
);
