using System.Text.Json.Serialization;

namespace Identity.Center.Application.Features.Authentication.Dtos;

public sealed record RouteDto(
  Guid Id,
  string Key,
  string Name,
  string Path,
  bool ExcludeNav,
  int Index,
  string? Icon,
  Guid? ParentId,
  IEnumerable<string> Claims,
  IEnumerable<RouteDto>? ChildRoutes = null
);
