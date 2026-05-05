namespace Identity.Center.Application.Common.Caching;

public sealed record TemporaryValue<T>(
  T Value,
  DateTimeOffset ExpiresAtUtc
);
