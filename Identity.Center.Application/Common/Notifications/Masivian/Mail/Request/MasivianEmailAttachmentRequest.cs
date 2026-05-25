namespace Identity.Center.Application.Common.Notifications.Masivian.Mail.Request;

public sealed record MasivianEmailAttachmentRequest(
  string Path,
  string FileName,
  MasivianEmailAttachmentActions Action,
  string? Password
);
