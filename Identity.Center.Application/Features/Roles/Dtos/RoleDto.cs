namespace Identity.Center.Application.Features.Roles.Dtos;

public sealed record RoleDto(
  Guid Id,
  Guid? AppId,
  string Name,
  string? Description,
  string? DomainName,
  bool Root,
  DateTimeOffset CreatedAtUtc
);
