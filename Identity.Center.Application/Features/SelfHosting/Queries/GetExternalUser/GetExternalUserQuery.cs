using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.SelfHosting.Dtos;

namespace Identity.Center.Application.Features.SelfHosting.Queries.GetExternalUser;

public sealed record GetExternalUserQuery(
  string? Filter,
  int? PageIndex,
  int? PageSize,
  bool? FullSet
) : IPaginationParams, IQuery<IPaginatedResult<OwnUserDto>>;
