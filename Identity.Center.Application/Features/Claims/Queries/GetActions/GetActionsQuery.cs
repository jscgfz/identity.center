using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Claims.Dtos;

namespace Identity.Center.Application.Features.Claims.Queries.GetActions;

public sealed record GetActionsQuery(
  string? Name,
  string? Description,
  int? PageIndex,
  int? PageSize,
  bool? FullSet
) : IPaginationParams, IQuery<IPaginatedResult<MasterClaimPart>>;
