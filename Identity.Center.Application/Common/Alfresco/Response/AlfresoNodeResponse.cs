using System.Text.Json;
using System.Text.Json.Serialization;
using Identity.Center.Application.Common.Serialization;

namespace Identity.Center.Application.Common.Alfresco.Response;

public sealed record AlfresoNodeResponse(
  string Id,
  string Name,
  string NodeType,
  bool IsFolder,
  bool IsFile,
  bool IsLocked,
  [property: JsonConverter(typeof(TimeZoneDatetimeOffsetConverter))] DateTimeOffset ModifiedAt,
  AlfrescoUserRefResponse ModifiedByUser,
  [property: JsonConverter(typeof(TimeZoneDatetimeOffsetConverter))] DateTimeOffset CreatedAt,
  AlfrescoUserRefResponse CreatedByUser,
  string? ParentId,
  bool? IsLink,
  bool? IsFavorite,
  bool? IsDirectLinkEnabled,
  AlfrescoContentInfoResponse? Content,
  IEnumerable<string>? AspectNames,
  Dictionary<string, JsonElement> Properties,
  IEnumerable<string>? AllowableOperations,
  AlfrescoPathInfoResponse? Path
);
