using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Claims.Dtos;

namespace Identity.Center.Application.Features.Claims.Queries.GetGroups;

public sealed record GetGroupsQuery(
  string? Name,
  string? Description,
  int? PageIndex,
  int? PageSize,
  bool? FullSet
) : IPaginationParams, IQuery<IPaginatedResult<MasterClaimPart>>;
