using Identity.Center.Application.Common.Response;

namespace Identity.Center.Application.Features.Authentication.Dtos;
public sealed record RoleAuthResponseDto(
  Guid Id,
  string Name,
  MasterOption<Guid> App
);
