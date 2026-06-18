namespace Identity.Center.Application.Common.Alfresco.Response;

public sealed record AlfrescoContentInfoResponse(
  string MimeType,
  string? MimeTypeName,
  double? SizeInBytes,
  string? Encoding
);
