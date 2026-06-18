using System.Linq;
using System.Net;
using System.Security.Claims;
using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.SelfHosting.Queries.GetRoutes;

internal sealed class GetRoutesQueryHandler(IServiceProvider provider) : IQueryHandler<GetRoutesQuery, IPaginatedResult<RouteConfigDto>>
{
  private readonly IIdentityRepository<AppRoute> _routeRepo = provider.GetRequiredService<IIdentityRepository<AppRoute>>();
  private readonly IHttpContextAccessor _context = provider.GetRequiredService<IHttpContextAccessor>();

  public async Task<Result<IPaginatedResult<RouteConfigDto>>> Handle(GetRoutesQuery request, CancellationToken cancellationToken)
  {
    if (
      _context.HttpContext == null ||
      _context.HttpContext.User.FindFirstValue(IdentityClaimTypes.App) is not string appClaim ||
      !Guid.TryParse(appClaim, out Guid appId)
    )
      return Result.Result.Failure<IPaginatedResult<RouteConfigDto>>(
        HttpStatusCode.Unauthorized,
        new BaseError("Invalid.Token", "Token invalido")
      );

    IPaginatedResult<RouteConfigDto> routes = await PaginatedResult
      .ComputeAsync(
        _routeRepo.Data
          .Where(row => row.AppId == appId)
          .Include($"{nameof(AppRoute.Claims)}.{nameof(RouteClaim.Claim)}.{nameof(RouteClaim.Claim.Group)}")
          .Include($"{nameof(AppRoute.Claims)}.{nameof(RouteClaim.Claim)}.{nameof(RouteClaim.Claim.Action)}")
          .OrderBy(row => row.ParentRouteId ?? row.Id)
          .ThenBy(row => row.ParentRouteId.HasValue)
          .Select(row => new RouteConfigDto(
            row.Id,
            row.Key,
            row.Name,
            row.Path,
            row.ExcludeNav,
            row.Index,
            row.Icon,
            row.ParentRouteId,
            row.Claims.Select(c => c.Claim.Group.Name + ":" + c.Claim.Action.Name)
          )),
        request,
        cancellationToken
      );

    return routes.AsResult();
  }
}
