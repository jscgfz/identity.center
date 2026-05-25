using System.Text.Json;
using Identity.Center.Domain.Common;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Identity.Center.Persistence.Common;

internal static class IdentityValueConverters
{
  public static ValueConverter<JsonElement, byte[]> JsonBytes => new(
    v => IdentityCommons.Encoding.GetBytes(JsonSerializer.Serialize(v, JsonSerializerOptions.Web)),
    v => JsonSerializer.Deserialize<JsonElement>(IdentityCommons.Encoding.GetString(v), JsonSerializerOptions.Web)
  );

  public static ValueConverter<TEnum, string> EnumJson<TEnum>()
    where TEnum : struct
    => new(v => IdentityCommons.Serialize(v), v => IdentityCommons.Deserialize<TEnum>(v));
}
