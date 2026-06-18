using System.Text.Json;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Authentication.Dtos;

namespace Identity.Center.Application.Features.Authentication.Queries.GetRoutes;

public sealed record GetRoutesQuery(
  bool Tree = true
) : IQuery<IEnumerable<RouteDto>>;
