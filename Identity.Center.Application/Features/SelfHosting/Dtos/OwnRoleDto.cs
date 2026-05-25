namespace Identity.Center.Application.Features.SelfHosting.Dtos;

public sealed record OwnRoleDto(
  Guid Id,
  string Name,
  string? Description,
  string? DomainName,
  bool ActiveDiretoryMandatory,
  bool Root,
  DateTimeOffset CreatedAtUtc,
  IEnumerable<string> Claims
);
