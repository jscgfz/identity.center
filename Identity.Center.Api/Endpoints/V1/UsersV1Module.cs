using Identity.Center.Api.Common;
using Identity.Center.Api.Configuration;
using Identity.Center.Api.Configuration.Authorization;
using Identity.Center.Api.Extensions;
using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Features.Credentials.Commands.AddCredentials;
using Identity.Center.Application.Features.Users.Commands.AddUser;
using Identity.Center.Application.Features.Users.Dtos;
using Identity.Center.Application.Features.Users.Queries.GetUsers;
using Identity.Center.Application.Features.Users.Queries.GetWholeUsers;
using Identity.Center.Application.Result;
using MediatR;

namespace Identity.Center.Api.Endpoints.V1;

public sealed class UsersV1Module : IIdentityModule
{
  public void Registry(IEndpointRouteBuilder builder)
  {
    RouteGroupBuilder group = builder
      .MapGroup("/users")
      .WithTags("users")
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Merged(
            BaseIdentityPolicies.Jwt,
            BaseIdentityPolicies.ApiKey
          )
      )
      .AddRequirement("autorization", "apikey")
      .AddRequirement("autorization", "jwt");

    group
      .MapGet(string.Empty, async ([AsParameters] GetWholeUsersQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireRoot()
          .RequireClaims("users:view")
      )
      .Produces<IPaginatedResult<BasicUserInfoDto>>()
      .AddRequirement("user", "root")
      .AddRequirement("claims", "user:view")
      .BuildRequirements("Obtiene la información del banco de usuarios entero (no discrimina aplicación)");

    group
      .MapGet("/apps", async ([AsParameters] GetUsersQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireClaims("users:view")
      )
      .AddRequirement("claims", "user:view")
      .BuildRequirements("Obtiene la información del banco de usuarios basandose en el contexto de la aplicación");

    group
      .MapPost(string.Empty, async (AddUserCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireRoot()
          .RequireClaims("users:create")
      )
      .AddRequirement("user", "root")
      .AddRequirement("claims", "user:create")
      .BuildRequirements("Crea un usuario en el sistema")
      .Produces<CreatedUserDto>();

    group
      .MapPost("/credentials", async (AddCredentialsCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .WithIdentityAuthorization(
        IdentityPolicyBuilder
          .Empty
          .RequireRoot()
          .RequireClaims("credentials:create")
      )
      .AddRequirement("user", "root")
      .AddRequirement("claims", "credentials:create")
      .BuildRequirements("Crea credenciales para un usuario en el sistema");
  }
}
