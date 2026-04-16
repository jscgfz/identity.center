using System.Text.Json;

namespace Identity.Center.Persistence.Common;

internal static class IdentityDefaultValues
{
  public const string UtcNow = "getutcdate()";
  public const string Guid = "newsequentialid()";
  public static JsonElement EmptyJson => JsonSerializer.Deserialize<JsonElement>("{}");
}
