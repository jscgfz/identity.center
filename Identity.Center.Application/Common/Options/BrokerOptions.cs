using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Identity.Center.Domain.Enums;

namespace Identity.Center.Application.Common.Options;

public sealed class BrokerOptions
{
  private const string TopicPrefix = "comms";
  private const string QueuePrefix = "queue";
  private static string Name(ContactTypes v)
  {
    Type type = typeof(ContactTypes);
    MemberInfo? memberInfo = type.GetMember(v.ToString()!).FirstOrDefault();
    return memberInfo is null
      ? throw new InvalidOperationException()
      : memberInfo.GetCustomAttribute<JsonStringEnumMemberNameAttribute>()?.Name ?? throw new InvalidOperationException();
  }
  public static string Topic(ContactTypes type) => $"{TopicPrefix}.{Name(type)}";
  public static string Queue(ContactTypes type) => $"{QueuePrefix}.{Name(type)}";
  public required string Host { get; set; }
  public required string Username { get; set; }
  public required string Password { get; set; }
  public required string VirtualHost { get; set; }
  public required string Exchange { get; set; }
}
