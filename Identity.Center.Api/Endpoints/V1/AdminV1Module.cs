using Identity.Center.Api.Configuration;
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
using Identity.Center.Domain.Enums;
using Identity.Center.Infrastructure.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Center.Api.Endpoints.V1;

public sealed class AdminV1Module /*: IIdentityModule*/
{
  public void Registry(IEndpointRouteBuilder builder)
  {
    RouteGroupBuilder group = builder
      .MapGroup("/admin")
      .RequireAuthorization(
        IdentityPolicies.Jwt,
        IdentityPolicies.ApiKey,
        IdentityPolicies.Root
      );

    MapAppRoutes(
      group
        .MapGroup("/apps")
        .WithTags("apps_admin")
    );

    MapClaimsRoutes(
      group
        .MapGroup("/claims")
    );

    MapApiKeysRoutes(
      group
        .MapGroup("/api-keys")
    );
  }

  private static void MapAppRoutes(RouteGroupBuilder group)
  {
    group.MapGet(
      string.Empty,
      async ([AsParameters] GetAppsQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
    )
      .Produces<IPaginatedResult<AppDto>>();

    group.MapPost(
      string.Empty,
      async (AddAppCommand command, ISender sender) => await sender.Send(command).AsHttpResult()
    )
      .Produces<CreatedAppDto>();

    group.MapGet(
      "/{appId}",
      async ([FromRoute] Guid appId, ISender sender) =>
        await sender.Send(new GetAppQuery(appId)).AsHttpResult()
    )
      .Produces<AppDto>();

    group.MapGet(
      "/{appId}/auth",
      async ([FromRoute] Guid appId, ISender sender) =>
        await sender.Send(new GetAppAuthQuery(appId)).AsHttpResult()
    )
      .Produces<AppAuthDto>();
  }

  private static void MapClaimsRoutes(RouteGroupBuilder group)
  {
    group
      .MapGet("/actions", async ([AsParameters] GetActionsQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .Produces<IPaginatedResult<MasterClaimPart>>();

    group
      .MapGet("/groups", async ([AsParameters] GetGroupsQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      );

    group
      .MapPost("/actions", async (AddActionsCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      );

    group
      .MapPost("/groups", async (AddGroupsCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      );
  }

  private static void MapApiKeysRoutes(RouteGroupBuilder group)
  {
    group
      .MapGet(string.Empty, async ([AsParameters] GetAdminApiKeysQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      );

    group
      .MapGet("/{subjectId}", async (Guid subjectId, ISender sender) =>
        await sender.Send(new GetAdminApiKeyQuery(subjectId)).AsHttpResult()
      );

    group
      .MapPost(string.Empty, async (AddApiKeyCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      );

    group
      .MapPost("/claims", async (AddApiKeyClaimsCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      );
  }
}
