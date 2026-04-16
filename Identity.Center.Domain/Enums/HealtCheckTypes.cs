using System.Text.Json.Serialization;

namespace Identity.Center.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HealtCheckTypes
{
  [JsonStringEnumMemberName("mssql")] SQLServer,
  [JsonStringEnumMemberName("postgre")] Postgre,
  [JsonStringEnumMemberName("mongodb")] Mongo,
  [JsonStringEnumMemberName("redis")] Redis,
  [JsonStringEnumMemberName("kafka")] Kafka,
  [JsonStringEnumMemberName("rabbitmq")] RabbitMQ
}
