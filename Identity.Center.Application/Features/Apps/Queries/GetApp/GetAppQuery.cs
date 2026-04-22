using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Apps.Dtos;

namespace Identity.Center.Application.Features.Apps.Queries.GetApp;

public sealed record GetAppQuery(
  Guid Id
) : IQuery<AppDto>;
