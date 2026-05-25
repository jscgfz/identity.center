using Identity.Center.Api.Configuration;
using Identity.Center.Api.Extensions;
using Identity.Center.Application.Features.Authentication.Commands.Login;
using Identity.Center.Application.Features.Authentication.Commands.MfaConfig;
using Identity.Center.Application.Features.Authentication.Commands.ValidateTotp;
using Identity.Center.Application.Features.Authentication.Dtos;
using Identity.Center.Application.Features.SelfHosting.Queries.GetConfig;
using Identity.Center.Application.Features.SelfHosting.Queries.GetOwnRoles;
using Identity.Center.Application.Features.SelfHosting.Queries.GetOwnUsers;
using Identity.Center.Application.Result;
using Identity.Center.Infrastructure.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
      .WithDescription("Inicio de sessión")
      .BuildRequirementsDoc("Inicio de sessión");

    group
      .MapPost("/auth/mfa/config", async ([FromServices] ISender sender) =>
        await sender.Send(new MfaConfigCommand()).AsHttpResult()
      )
      .RequireAuthorization(
        IdentityPolicies.MfaPending
      )
      .BuildRequirementsDoc("Obtiene la configuración del TOTP");

    group
      .MapPost("/auth/mfa", async (ValidateTotpCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .Produces<AuthenticationReponseDto>()
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
      .BuildRequirementsDoc("Obtiene las variables de configuración de la aplicación");

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
      .BuildRequirementsDoc("Obtiene los roles de la aplicación");

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
      .BuildRequirementsDoc("Obtiene los roles de la aplicación");
  }
}
