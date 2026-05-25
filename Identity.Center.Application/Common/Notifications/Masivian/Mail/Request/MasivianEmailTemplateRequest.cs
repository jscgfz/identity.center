namespace Identity.Center.Application.Common.Notifications.Masivian.Mail.Request;

public sealed record MasivianEmailTemplateRequest(
  MasivianEmailParameterTypes Type,
  string Value
);
