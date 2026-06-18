using System.Text.Json.Serialization;

namespace Identity.Center.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChangeControlStates
{
  [JsonStringEnumMemberName("pending")] Pending,
  [JsonStringEnumMemberName("rejected")] Rejected,
  [JsonStringEnumMemberName("approved")] Approved
}
