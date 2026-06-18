namespace Identity.Center.Application.Common.Options;

public sealed class AlfrescoOptions
{
  public required string BaseUrl { get; set; }
  public required string Username { get; set; }
  public required string Password { get; set; }
  public required Dictionary<string, string> NodeCollection { get; set; }
  public required IEnumerable<string> ValidMimeTypes { get; set; }
}
