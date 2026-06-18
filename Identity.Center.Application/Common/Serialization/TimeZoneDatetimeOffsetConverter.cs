using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Identity.Center.Application.Common.Serialization;

public sealed class TimeZoneDatetimeOffsetConverter : JsonConverter<DateTimeOffset>
{
  [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] private readonly string _format = "yyyy-MM-dd'T'hh:mm:ss.fffzz00";
  public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    => DateTimeOffset.Parse(reader.GetString()!);

  public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
  {
    writer
      .WriteStringValue(
        value.ToString(_format)
      );
  }
}
