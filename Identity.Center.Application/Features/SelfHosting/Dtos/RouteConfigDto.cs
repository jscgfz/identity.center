namespace Identity.Center.Application.Features.SelfHosting.Dtos;

public sealed record RouteConfigDto(
  Guid Id,
  string Key,
  string Name,
  string Path,
  bool ExcludeNav,
  int Index,
  string? Icon,
  Guid? ParentId,
  IEnumerable<string> Claims
);

