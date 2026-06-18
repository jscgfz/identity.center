using Identity.Center.Api.Configuration;
using Identity.Center.Api.Extensions;
using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Features.Authentication.Commands.Login;
using Identity.Center.Application.Features.Authentication.Commands.MfaConfig;
using Identity.Center.Application.Features.Authentication.Commands.ValidateTotp;
using Identity.Center.Application.Features.Authentication.Dtos;
using Identity.Center.Application.Features.Authentication.Queries.GetRoutes;
using Identity.Center.Application.Features.SelfHosting.Commands.AddRequestRole;
using Identity.Center.Application.Features.SelfHosting.Commands.AddUser;
using Identity.Center.Application.Features.SelfHosting.Commands.ModifyRole;
using Identity.Center.Application.Features.SelfHosting.Commands.ModifyUser;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Identity.Center.Application.Features.SelfHosting.Queries.GetConfig;
using Identity.Center.Application.Features.SelfHosting.Queries.GetExternalUser;
using Identity.Center.Application.Features.SelfHosting.Queries.GetOwnRoles;
using Identity.Center.Application.Features.SelfHosting.Queries.GetOwnUsers;
using Identity.Center.Application.Features.SelfHosting.Queries.GetRequestRoles;
using Identity.Center.Application.Result;
using Identity.Center.Infrastructure.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using GetRoutesQuery2 = Identity.Center.Application.Features.SelfHosting.Queries.GetRoutes.GetRoutesQuery;

namespace Identity.Center.Api.Endpoints.V1;

public sealed class SelfHostingModule : IIdentityModule
{
  public void Registry(IEndpointRouteBuilder builder)
  {
    RouteGroupBuilder group = builder
      .MapGroup("/-self-")
      .WithTags("self_hosting");

    group
      .MapPost("/{appId}/auth/login", async ([FromRoute] Guid appId, [FromBody] LoginRequestDto dto, ISender sender) =>
        await sender.Send(new LoginCommand(appId, dto.Username, dto.Password)).AsHttpResult()
      )
      .AllowAnonymous()
      .Produces<AuthenticationReponseDto>()
      .AddAuthProduces()
      .BuildRequirementsDoc("Inicio de sessión");

    group
      .MapGet("/auth/routes", async ([AsParameters] GetRoutesQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.Jwt
      )
      .Produces<IEnumerable<RouteDto>>()
      .AddAuthProduces()
      .BuildRequirementsDoc("Obtiene las rutas accesibles de la aplicación");

    group
      .MapPost("/auth/mfa/config", async ([FromServices] ISender sender) =>
        await sender.Send(new MfaConfigCommand()).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.MfaPending
      )
      .Produces<FileStreamResult>(contentType: "image/png")
      .AddAuthProduces()
      .BuildRequirementsDoc("Obtiene la configuración del TOTP");

    group
      .MapPost("/auth/mfa", async (ValidateTotpCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .Produces<AuthenticationReponseDto>()
      .AddAuthProduces()
      .RequireAuthorization(
        IdentityPolicies.MfaPending
      )
      .BuildRequirementsDoc("Verifica la TOTP y valida el token");

    group
      .MapGet("/config", async ([AsParameters] GetConfigQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.ApiKey,
        IdentityPolicies.Root,
        IdentityPolicies.FromClaim("config:view")
      )
      .Produces<IEnumerable<KeyValuePair<string, string>>>()
      .AddAuthProduces()
      .BuildRequirementsDoc("Obtiene las variables de configuración de la aplicación");


    group
      .MapGet("/routes", async ([AsParameters] GetRoutesQuery2 query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.Jwt
      )
      .Produces<IEnumerable<RouteDto>>()
      .AddAuthProduces()
      .BuildRequirementsDoc("Obtiene las rutas de la aplicación");

    group
      .MapGet("/roles", async ([AsParameters] GetOwnRolesQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.ApiKey,
        IdentityPolicies.Jwt,
        IdentityPolicies.Root,
        IdentityPolicies.FromClaim("roles:view")
      )
      .Produces<IPaginatedResult<OwnRoleDto>>()
      .AddAuthProduces()
      .BuildRequirementsDoc("Obtiene los roles de la aplicación");

    group
      .MapPut("/roles", async (
        [FromForm] Guid roleId,
        [FromForm] ModifyRoleRequestDto dto,
        ISender sender) =>
        await sender.Send(new AddRequestRoleCommand(roleId, dto)).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.ApiKey,
        IdentityPolicies.Jwt,
        IdentityPolicies.Root,
        IdentityPolicies.FromClaim("roles:request")
      )
      .DisableAntiforgery()
      .Produces<IPaginatedResult<OwnRoleDto>>()
      .AddAuthProduces()
      .BuildRequirementsDoc("Solicita una edición de rol en el sistema");

    group
      .MapGet("/roles/requests", async ([AsParameters] GetRequestRolesQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.ApiKey,
        IdentityPolicies.Jwt,
        IdentityPolicies.Root,
        IdentityPolicies.FromClaim("roles:request")
      )
      .Produces<IPaginatedResult<RolePictureComparisonDto>>()
      .AddAuthProduces()
      .BuildRequirementsDoc("Obtiene la historia de solicitudes de cambio del sistema");

    group
      .MapPut("/roles/requests", async (ModifyRoleCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.ApiKey,
        IdentityPolicies.Jwt,
        IdentityPolicies.Root,
        IdentityPolicies.FromClaim("roles:update")
      )
      .Produces<ModifiedUserDto>()
      .AddAuthProduces()
      .BuildRequirementsDoc("Confirma o rechaza los cambios relacionados en la solicitud de edición de roles");

    group
      .MapGet("/users", async ([AsParameters] GetOwnUsersQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.ApiKey,
        IdentityPolicies.Jwt,
        IdentityPolicies.Root,
        IdentityPolicies.FromClaim("users:view")
      )
      .Produces<IPaginatedResult<OwnUserDto>>()
      .AddAuthProduces()
      .BuildRequirementsDoc("Obtiene los usuarios de la aplicación");

    group
      .MapPost("/users", async (AddUserCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.ApiKey,
        IdentityPolicies.Jwt,
        IdentityPolicies.Root,
        IdentityPolicies.FromClaim("users:create")
      )
      .Produces<CreatedUserDto>()
      .AddAuthProduces()
      .BuildRequirementsDoc("Crea un usuario en el sistema y lo relaciona a la aplicación");

    group
      .MapPut("/users", async ([FromBody] ModifyUserCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.ApiKey,
        IdentityPolicies.Jwt,
        IdentityPolicies.Root,
        IdentityPolicies.FromClaim("users:update")
      )
      .AddAuthProduces()
      .BuildRequirementsDoc("Modifica un usuario de identity");

    group
      .MapGet("/users/ext", async ([AsParameters] GetExternalUserQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.ApiKey,
        IdentityPolicies.Jwt,
        IdentityPolicies.Root,
        IdentityPolicies.FromClaim("users:external")
      )
      .Produces<IPaginatedResult<OwnUserDto>>()
      .AddAuthProduces()
      .BuildRequirementsDoc("Obtiene los usuarios que no estan en la aplicación pero están registrados en identity");
  }
}
