namespace Identity.Center.Application.Common.Notifications.Masivian.Sms.Request;

public sealed record MasivianSmsRequest(
  string To,
  string? Text,
  int? IdTemplate,
  string? CustomData,
  bool IsPremium = false,
  bool IsFlash = true,
  bool? IsRandomRoute = null,
  MasivianSmsShortUrlRequest? ShortUrlConfig = null,
  Dictionary<string, string>? ReplacementFields = null
)
{
  public bool IsLongMessage => !string.IsNullOrEmpty(Text) &&
    (
      (!IsPremium && Text.Length > 160) ||
      (IsPremium && Text.Length > 70)
    );
}
