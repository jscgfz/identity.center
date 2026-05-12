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
      )
      .AddRequirement("user", "root");

    MapAppRoutes(
      group
        .MapGroup("/apps")
        .WithTags("apps_admin")
        .WithIdentityAuthorization(
          BaseIdentityPolicies.ApiKey
        )
        .AddRequirement("authorization", "apikey")
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
        .WithTags("claims_admin")
        .AddRequirement("authorization", "apikey")
        .AddRequirement("authorization", "jwt")
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
        .WithTags("apikey_admin")
        .AddRequirement("authorization", "apikey")
        .AddRequirement("authorization", "jwt")
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
      .AddRequirement("claims", "apps:view")
      .BuildRequirements("Obtiene la paginación de las aplicaciones")
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
      .AddRequirement("claims", "apps:create")
      .BuildRequirements("Crea una aplicación")
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
      .AddRequirement("claims", "apps:view")
      .BuildRequirements("Obtiene la informacón de una aplicaciín concreta")
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
      .AddRequirement("claims", "apps:view")
      .BuildRequirements("Obtiene la información de autenticación de una aplicación")
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
      .AddRequirement("claims", "actions:view")
      .BuildRequirements("Obtiene las acciones de los claims del sistema")
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
      .AddRequirement("claims", "groups:view")
      .BuildRequirements("Obtiene los grupos de los claims del sistema");

    group
      .MapPost("/actions", async (AddActionsCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("actions:create")
      )
      .AddRequirement("claims", "actions:create")
      .BuildRequirements("Crea nuevas acciones para configurar claims del sistema");

    group
      .MapPost("/groups", async (AddGroupsCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("groups:create")
      )
      .AddRequirement("claims", "groups:create")
      .BuildRequirements("Crea nuevos grupos para configurar claims del sistema");
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
      .AddRequirement("claims", "apikeys:view")
      .BuildRequirements("Obtiene información de las api keys creadas en el sistema");

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
      .AddRequirement("claims", "apikeys:view")
      .BuildRequirements("Obtiene información de una api key especifica");

    group
      .MapPost(string.Empty, async (AddApiKeyCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("apikeys:create")
      )
      .AddRequirement("claims", "apikeys:create")
      .Produces<CreatedApkiKeyDto>()
      .BuildRequirements("Crea un api key para el consumo del sistema");

    group
      .MapPost("/claims", async (AddApiKeyClaimsCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("apikeys:create")
      )
      .AddRequirement("claims", "apikeys:create")
      .Produces<RelatedClaimDto>()
      .BuildRequirements("Agrega claims a una api key del sistema");
  }
}
