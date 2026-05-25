using Identity.Center.Application.Abstractions.Reponses;

namespace Identity.Center.Application.Common;

public sealed record BaseFileRender(
  byte[] Content,
  string Name,
  string MimeType
) : IFileResponse
{
  public FileContentResponse Render()
    => new(
      Content,
      Name,
      MimeType
    );
}
