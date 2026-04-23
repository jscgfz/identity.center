using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Apps.Dtos;

namespace Identity.Center.Application.Features.Apps.Queries.GetAppAuth;

public sealed record GetAppAuthQuery(
  Guid Id
) : IQuery<AppAuthDto>;
