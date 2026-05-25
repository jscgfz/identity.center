using Identity.Center.Domain.Enums;

namespace Identity.Center.Application.Features.Authentication.Dtos;

public sealed record AuthenticationReponseDto(
  Guid Id,
  string Name,
  DateTimeOffset IssuedAtUtc,
  DateTimeOffset ExpiresAtUtc,
  IEnumerable<RoleAuthResponseDto> Roles,
  MfaStates Mfa
);
