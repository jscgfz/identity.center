using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.SelfHosting.Dtos;

namespace Identity.Center.Application.Features.SelfHosting.Queries.GetOwnRoles;

public sealed record GetOwnRolesQuery(
  int? PageIndex,
  int? PageSize,
  bool? FullSet,
  string? Name,
  string? Description,
  string? DomainName,
  bool? Root,
  string? Claim
) : IPaginationParams, IQuery<IPaginatedResult<OwnRoleDto>>;
