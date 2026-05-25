using System.Reflection;
using System.Text.Json.Serialization;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Enums;

namespace Identity.Center.Application.Common.Options;

public sealed class BrokerOptions
{
  private const string TopicPrefix = "comms";
  private const string QueuePrefix = "queue";
  public static string Topic(ContactTypes type) => $"{TopicPrefix}.{IdentityCommons.Serialize(type)}";
  public static string Queue(ContactTypes type) => $"{QueuePrefix}.{IdentityCommons.Serialize(type)}";
  public required string Host { get; set; }
  public required string Username { get; set; }
  public required string Password { get; set; }
  public required string VirtualHost { get; set; }
  public required string Exchange { get; set; }
}
