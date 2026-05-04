using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.ApiKeys.Dtos;

namespace Identity.Center.Application.Features.ApiKeys.Queries.GetAdminApiKeys;

public sealed record GetAdminApiKeysQuery(
  string? Subject,
  string? Name,
  string? Description,
  string? App,
  string? Claims,
  int? PageIndex,
  int? PageSize,
  bool? FullSet
) : IPaginationParams, IQuery<IPaginatedResult<ApiKeyDto>>;
