namespace Identity.Center.Application.Features.Apps.Dtos;

public sealed record AppAuthDto(
  Guid Id,
  byte[] SignatureKey,
  bool TwoFactorEnabled,
  TimeSpan ExpirationTime,
  TimeSpan RefreshTime,
  DateTimeOffset CreatedAtUtc
);