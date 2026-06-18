using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Authentication.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Authentication.Queries.GetRoutes;

internal sealed class GetRoutesQueryHandler(IServiceProvider provider) : IQueryHandler<GetRoutesQuery, IEnumerable<RouteDto>>
{
  private readonly IIdentityRepository<AppRoute> _routeRepo = provider.GetRequiredService<IIdentityRepository<AppRoute>>();
  private readonly IHttpContextAccessor _context = provider.GetRequiredService<IHttpContextAccessor>();

  public async Task<Result<IEnumerable<RouteDto>>> Handle(GetRoutesQuery request, CancellationToken cancellationToken)
  {
    if (
      _context.HttpContext == null ||
      _context.HttpContext.User.FindFirstValue(IdentityClaimTypes.App) is not string appClaim ||
      !Guid.TryParse(appClaim, out Guid appId)
    )
      return Result.Result.Failure<IEnumerable<RouteDto>>(
        HttpStatusCode.Unauthorized,
        new BaseError("Invalid.Token", "Token invalido")
      );

    IEnumerable<string> userClaims = _context.HttpContext.User.FindAll(IdentityClaimTypes.Caim)
      .Select(c => c.Value);

    IEnumerable<RouteDto> routes = await _routeRepo.Data
      .Where(row => row.AppId == appId && row.Claims.Any(c => userClaims.Contains(c.Claim.Group.Name + ":" + c.Claim.Action.Name)))
      .Include($"{nameof(AppRoute.Claims)}.{nameof(RouteClaim.Claim)}.{nameof(RouteClaim.Claim.Group)}")
      .Include($"{nameof(AppRoute.Claims)}.{nameof(RouteClaim.Claim)}.{nameof(RouteClaim.Claim.Action)}")
      .Select(row => new RouteDto(
        row.Id,
        row.Key,
        row.Name,
        row.Path,
        row.ExcludeNav,
        row.Index,
        row.Icon,
        row.ParentRouteId,
        row.Claims.Where(c => userClaims.Contains(c.Claim.Group.Name + ":" + c.Claim.Action.Name))
          .Select(c => c.Claim.Group.Name + ":" + c.Claim.Action.Name),
        null
      ))
      .ToListAsync(cancellationToken);

    if(!routes.Any())
      return Result.Result.Failure<IEnumerable<RouteDto>>(
        HttpStatusCode.NotFound,
        new BaseError("Routes.NotFound", "No se encontraron rutas")
      );

    return request.Tree ? Reduce(routes).Success() : routes.Success();
  }

  private static IEnumerable<RouteDto> Reduce(IEnumerable<RouteDto> routes, Guid? parentId = null)
  {
    foreach(RouteDto route in routes.Where(r => r.ParentId == parentId))
    {
      RouteDto routeCopy = route;
      if (routes.Any(r => r.ParentId == routeCopy.Id))
        routeCopy = routeCopy with
        {
          ChildRoutes = Reduce(routes, routeCopy.Id)
        };

      yield return routeCopy;
    }
  }
}
