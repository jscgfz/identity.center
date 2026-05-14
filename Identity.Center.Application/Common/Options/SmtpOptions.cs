namespace Identity.Center.Application.Common.Options;

public sealed class SmtpOptions
{
  public required string Host { get; set; }
  public required int Port { get; set; }
  public required string Username { get; set; }
  public required string Password { get; set; }
}
