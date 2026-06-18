namespace Identity.Center.Application.Common.Alfresco.Response;

public sealed record AlfrescoPathElementResponse(
  string? Id,
  string? Name,
  string? NodeType,
  IEnumerable<string>? AspectNames
);
