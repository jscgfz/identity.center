using Identity.Center.Api.Configuration;
using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Features.Apps.Commands.AddApp;
using Identity.Center.Application.Features.Apps.Dtos;
using Identity.Center.Application.Features.Apps.Queries.GetApp;
using Identity.Center.Application.Features.Apps.Queries.GetAppAuth;
using Identity.Center.Application.Features.Apps.Queries.GetApps;
using Identity.Center.Application.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Center.Api.Endpoints.V1;

public sealed class AppsV1Module : IIdentityModule
{
  public void Registry(IEndpointRouteBuilder builder)
  {
    RouteGroupBuilder group = builder
      .MapGroup("/apps")
      .WithTags("Aplicaciones")
      .WithDescription("Gestión de aplicaciones");

    group.MapGet(
      string.Empty,
      async ([AsParameters] GetAppsQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
    )
      .WithDescription("Obtiene la paginación de las aplicaciones")
      .Produces<IPaginatedResult<AppDto>>();

    group.MapPost(
      string.Empty,
      async (AddAppCommand command, ISender sender) => await sender.Send(command).AsHttpResult()
    )
      .WithDescription("Crea una aplicación")
      .Produces<CreatedAppDto>();

    group.MapGet(
      "/{appId}",
      async ([FromRoute] Guid appId, ISender sender) =>
        await sender.Send(new GetAppQuery(appId)).AsHttpResult()
    )
      .WithDescription("Obtiene la informacón de una aplicaciín concreta")
      .Produces<AppDto>();

    group.MapGet(
      "/{appId}/auth",
      async ([FromRoute] Guid appId, ISender sender) =>
        await sender.Send(new GetAppAuthQuery(appId)).AsHttpResult()
    )
      .WithDescription("Obtiene la información de autenticación de una aplicación")
      .Produces<AppAuthDto>();
  }
}
