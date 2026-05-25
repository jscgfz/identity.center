namespace Identity.Center.Application.Common.Notifications.Masivian.Sms.Request;

public sealed record MasivianSmsShortUrlRequest(
  string Url,
  string? DomainShortUrl = null
);
