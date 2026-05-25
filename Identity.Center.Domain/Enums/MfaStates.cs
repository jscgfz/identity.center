using System.Text.Json.Serialization;

namespace Identity.Center.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MfaStates
{
  [JsonStringEnumMemberName("pending")] Pending,
  [JsonStringEnumMemberName("passed")] Passed,
  [JsonStringEnumMemberName("not-required")] NotRequired
}
