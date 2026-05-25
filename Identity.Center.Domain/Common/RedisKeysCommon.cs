namespace Identity.Center.Domain.Common;

public static class RedisKeysCommon
{
  public static string ApiKeyHashKey(Guid apiKey) => $"apikeys:{apiKey:N}";
  public static string RoleHashKey(Guid roleId) => $"roles:{roleId}";
  public static string SessionHashKey(Guid userId) => $"sessions:{userId}";
  public static string ClaimsStore => "id:claims";
}
