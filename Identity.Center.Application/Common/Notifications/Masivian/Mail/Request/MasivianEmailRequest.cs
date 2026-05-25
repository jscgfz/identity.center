namespace Identity.Center.Application.Common.Notifications.Masivian.Mail.Request;

public sealed record MasivianEmailRequest(
  string Subject,
  string From,
  IEnumerable<MasivianEmailRecipientRequest> Recipients,
  MasivianEmailTemplateRequest Template,
  string? ReplyTo = null,
  IEnumerable<MasivianEmailParameterRequest>? Parameters = null,
  IEnumerable<MasivianEmailAttachmentRequest>? Attachments = null,
  Dictionary<string, string>? Metadata = null
);
