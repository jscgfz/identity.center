using Identity.Center.Domain.Enums;

namespace Identity.Center.Application.Features.Users.Dtos;
public sealed record ContactInfoRequestDto(
  ContactTypes ContactTypeId,
  string Value
);
