using Identity.Center.Domain.Enums;

namespace Identity.Center.Application.Features.Users.Dtos;

public sealed record ContactInfoDto(
  Guid Id,
  Guid? UserId,
  ContactTypes TypeId,
  string Value,
  bool Confirmed,
  DateTimeOffset CreatedAtUtc
);
