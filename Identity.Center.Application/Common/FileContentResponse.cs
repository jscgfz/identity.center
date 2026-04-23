namespace Identity.Center.Application.Common;
public sealed record FileContentResponse(
  byte[] Content,
  string Name,
  string MimeType
);
