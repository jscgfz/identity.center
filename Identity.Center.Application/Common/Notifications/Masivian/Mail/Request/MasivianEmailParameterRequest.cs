namespace Identity.Center.Application.Common.Notifications.Masivian.Mail.Request;

public sealed record MasivianEmailParameterRequest(
  string Name,
  MasivianEmailParameterTypes Type,
  string Value
);
