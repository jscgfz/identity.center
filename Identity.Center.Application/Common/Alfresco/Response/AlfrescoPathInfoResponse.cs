namespace Identity.Center.Application.Common.Alfresco.Response;

public sealed record AlfrescoPathInfoResponse(
  IEnumerable<AlfrescoPathElementResponse>? Elements,
  string? Name,
  bool? IsComplete
);
