using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Apps.Dtos;

namespace Identity.Center.Application.Features.Apps.Queries.GetApps;

public sealed record GetAppsQuery(
  int? PageIndex,
  int? PageSize,
  bool? FullSet
) : IPaginationParams, IQuery<IPaginatedResult<AppDto>>;
