using Identity.Center.Application.Features.Apps.Dtos;

namespace Identity.Center.Application.Features.ApiKeys.Dtos;

public sealed record ApiKeyDto(
  Guid SubjectId,
  string Name,
  string? Description,
  AppDto? App,
  DateTimeOffset CreatedAtUtc,
  IEnumerable<string> Claims
);
