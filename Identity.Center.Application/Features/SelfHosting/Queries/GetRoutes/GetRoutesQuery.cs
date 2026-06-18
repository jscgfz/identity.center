using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.SelfHosting.Dtos;

namespace Identity.Center.Application.Features.SelfHosting.Queries.GetRoutes;

public sealed record GetRoutesQuery(
  string? Name,
  int? PageIndex,
  int? PageSize,
  bool? FullSet
) : IPaginationParams, IQuery<IPaginatedResult<RouteConfigDto>>;