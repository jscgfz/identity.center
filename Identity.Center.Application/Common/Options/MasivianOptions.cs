namespace Identity.Center.Application.Common.Options;

public sealed class MasivianOptions
{
  public required string EmailBaseUrl { get; set; }
  public required string SmsBaseUrl { get; set; }
  public required string Username { get; set; }
  public required string Password { get; set; }
  public required string Sender { get; set; }
}
