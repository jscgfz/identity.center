using Identity.Center.Api.Common;
using Identity.Center.Api.Configuration;
using Identity.Center.Api.Configuration.Authorization;
using Identity.Center.Api.Extensions;
using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Features.ApiKeys.Commands.AddApiKey;
using Identity.Center.Application.Features.ApiKeys.Dtos;
using Identity.Center.Application.Features.ApiKeys.Queries.GetAdminApiKey;
using Identity.Center.Application.Features.ApiKeys.Queries.GetAdminApiKeys;
using Identity.Center.Application.Features.Apps.Commands.AddApp;
using Identity.Center.Application.Features.Apps.Dtos;
using Identity.Center.Application.Features.Apps.Queries.GetApp;
using Identity.Center.Application.Features.Apps.Queries.GetAppAuth;
using Identity.Center.Application.Features.Apps.Queries.GetApps;
using Identity.Center.Application.Features.Claims.Commands.AddActions;
using Identity.Center.Application.Features.Claims.Commands.AddApiKeyClaims;
using Identity.Center.Application.Features.Claims.Commands.AddGroups;
using Identity.Center.Application.Features.Claims.Dtos;
using Identity.Center.Application.Features.Claims.Queries.GetActions;
using Identity.Center.Application.Features.Claims.Queries.GetGroups;
using Identity.Center.Application.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Center.Api.Endpoints.V1;

public sealed class AdminV1Module : IIdentityModule
{
  public void Registry(IEndpointRouteBuilder builder)
  {
    RouteGroupBuilder group = builder
      .MapGroup("/admin")
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireRoot()
      );

    MapAppRoutes(
      group
        .MapGroup("/apps")
        .WithTags("Administración apps")
        .WithIdentityAuthorization(
          BaseIdentityPolicies.ApiKey
        )
        .WithDescription("administración de las apps del sistema")
    );

    MapClaimsRoutes(
      group
        .MapGroup("/claims")
        .WithIdentityAuthorization(
          IdentityPolicyBuilder
            .Merged(
              BaseIdentityPolicies.ApiKey,
              BaseIdentityPolicies.Jwt
            )
        )
        .WithTags("Administración claims")
        .WithDescription("administración de los claims del sistema")
    );

    MapApiKeysRoutes(
      group
        .MapGroup("/api-keys")
        .WithIdentityAuthorization(
          IdentityPolicyBuilder
            .Merged(
              BaseIdentityPolicies.ApiKey,
              BaseIdentityPolicies.Jwt
            )
        )
        .WithTags("Administración api keys")
        .WithDescription("administración de las api keys del sistema")
    );
  }

  private static void MapAppRoutes(RouteGroupBuilder group)
  {
    group.MapGet(
      string.Empty,
      async ([AsParameters] GetAppsQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
    )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("apps:view")
      )
      .WithDescription("Obtiene la paginación de las aplicaciones")
      .Produces<IPaginatedResult<AppDto>>();

    group.MapPost(
      string.Empty,
      async (AddAppCommand command, ISender sender) => await sender.Send(command).AsHttpResult()
    )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("apps:create")
      )
      .WithDescription("Crea una aplicación")
      .Produces<CreatedAppDto>();

    group.MapGet(
      "/{appId}",
      async ([FromRoute] Guid appId, ISender sender) =>
        await sender.Send(new GetAppQuery(appId)).AsHttpResult()
    )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("apps:view")
      )
      .WithDescription("Obtiene la informacón de una aplicaciín concreta")
      .Produces<AppDto>();

    group.MapGet(
      "/{appId}/auth",
      async ([FromRoute] Guid appId, ISender sender) =>
        await sender.Send(new GetAppAuthQuery(appId)).AsHttpResult()
    )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("apps:view")
      )
      .WithDescription("Obtiene la información de autenticación de una aplicación")
      .Produces<AppAuthDto>();
  }

  private static void MapClaimsRoutes(RouteGroupBuilder group)
  {
    group
      .MapGet("/actions", async ([AsParameters] GetActionsQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("actions:view")
      )
      .WithDescription("Obtiene las acciones de los claims del sistema")
      .Produces<IPaginatedResult<MasterClaimPart>>();

    group
      .MapGet("/groups", async ([AsParameters] GetGroupsQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("groups:view")
      )
      .WithDescription("Obtiene los grupos de los claims del sistema");

    group
      .MapPost("/actions", async (AddActionsCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("actions:create")
      )
      .WithDescription("Crea nuevas acciones para configurar claims del sistema");

    group
      .MapPost("/groups", async (AddGroupsCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("groups:create")
      )
      .WithDescription("Crea nuevos grupos para configurar claims del sistema");
  }

  private static void MapApiKeysRoutes(RouteGroupBuilder group)
  {
    group
      .MapGet(string.Empty, async ([AsParameters] GetAdminApiKeysQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("apikeys:view")
      )
      .Produces<IPaginatedResult<ApiKeyDto>>()
      .WithDescription("Obtiene información de las api keys creadas en el sistema");

    group
      .MapGet("/{subjectId}", async (Guid subjectId, ISender sender) =>
        await sender.Send(new GetAdminApiKeyQuery(subjectId)).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("apikeys:view")
      )
      .Produces<ApiKeyDto>()
      .WithDescription("Obtiene información de una api key especifica");

    group
      .MapPost(string.Empty, async (AddApiKeyCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("apikeys:create")
      )
      .Produces<CreatedApkiKeyDto>()
      .WithDescription("Crea un api key para el consumo del sistema");

    group
      .MapPost("/claims", async (AddApiKeyClaimsCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("apikeys:create")
      )
      .Produces<RelatedClaimDto>()
      .WithDescription("Agrega claims a una api key del sistema");
  }
}
