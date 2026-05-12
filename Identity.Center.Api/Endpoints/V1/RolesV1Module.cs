using Identity.Center.Api.Configuration;
using Identity.Center.Application.Features.Roles.Commands.AddRole;
using Identity.Center.Application.Features.Roles.Dtos;
using Identity.Center.Application.Result;
using MediatR;

namespace Identity.Center.Api.Endpoints.V1;

public sealed class RolesV1Module : IIdentityModule
{
  public void Registry(IEndpointRouteBuilder builder)
  {
    RouteGroupBuilder group = builder
      .MapGroup("/roles")
      .WithTags("roles");

    group
      .MapPost(string.Empty, async (AddRoleCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .Produces<CreatedRoleDto>();
  }
}
