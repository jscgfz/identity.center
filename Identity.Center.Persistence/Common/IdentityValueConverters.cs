using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
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
  {
    Func<TEnum, string> toString = v =>
    {
      Type type = typeof(TEnum);
      MemberInfo? memberInfo = type.GetMember(v.ToString()!).FirstOrDefault();
      return memberInfo is null
        ? throw new InvalidOperationException()
        : memberInfo.GetCustomAttribute<JsonStringEnumMemberNameAttribute>()?.Name ?? throw new InvalidOperationException();
    };

    Func<string, TEnum> toEnum = v =>
    {
      Type type = typeof(TEnum);
      MemberInfo? memberInfo = type.GetMembers()
        .FirstOrDefault(mi => mi.GetCustomAttribute<JsonStringEnumMemberNameAttribute>() is JsonStringEnumMemberNameAttribute attr && attr.Name.Equals(v));
      return memberInfo is null
        ? throw new InvalidOperationException()
        : Enum.Parse<TEnum>(memberInfo.Name);
    };

    return new(v => toString(v), v => toEnum(v));
  }
}
