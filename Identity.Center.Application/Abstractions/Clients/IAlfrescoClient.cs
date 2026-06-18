using Refit;

namespace Identity.Center.Application.Abstractions.Clients;

public interface IAlfrescoClient
{
  //[Post("​/nodes/{nodeId}/children")]
  [Post("/nodes/{nodeId}/children")]
  [Multipart]
  Task<HttpResponseMessage> PostChildren(
    string nodeId,
    [AliasAs("name")] string name,
    [AliasAs("filedata")] StreamPart filedata,
    [AliasAs("relativePath")] string? relativePath = null,
    [AliasAs("nodeType")] string content = "cm:content",
    [AliasAs("overwrite")] string overwrite = "false",
    [AliasAs("cm:title")] string? title = null,
    [AliasAs("cm:description")] string? description = null
  );
}