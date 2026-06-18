using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Identity.Center.Domain.Enums;

namespace Identity.Center.Application.Features.SelfHosting.Queries.GetRequestRoles;

public sealed record GetRequestRolesQuery(
  int? PageIndex,
  int? PageSize,
  bool? FullSet,
  ChangeControlStates? Status
) : IPaginationParams, IQuery<IPaginatedResult<RolePictureComparisonDto>>;
