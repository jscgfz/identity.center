using Microsoft.Identity.Client;

namespace Identity.Center.Application.Common.Alfresco.Response;

public sealed record AlfrescoNodeListPaginationResponse(
  int Count,
  bool HasMoreItems,
  int TotalItems,
  int SkipCount,
  int MaxItems
);
