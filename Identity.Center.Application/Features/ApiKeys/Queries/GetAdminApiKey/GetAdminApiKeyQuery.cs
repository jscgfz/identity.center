using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.ApiKeys.Dtos;

namespace Identity.Center.Application.Features.ApiKeys.Queries.GetAdminApiKey;

public sealed record GetAdminApiKeyQuery(
  Guid SubjectId
) : IQuery<ApiKeyDto>;
