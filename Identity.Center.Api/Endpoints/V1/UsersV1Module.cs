using Identity.Center.Application.Features.Credentials.Commands.AddCredentials;
using Identity.Center.Application.Features.Users.Commands.AddUser;
using Identity.Center.Application.Features.Users.Dtos;
using Identity.Center.Application.Features.Users.Queries.GetUsers;
using Identity.Center.Application.Features.Users.Queries.GetWholeUsers;
using Identity.Center.Application.Result;
using MediatR;

namespace Identity.Center.Api.Endpoints.V1;

public sealed class UsersV1Module /*: IIdentityModule*/
{
  public void Registry(IEndpointRouteBuilder builder)
  {
    RouteGroupBuilder group = builder
      .MapGroup("/users")
      .WithTags("users");

    group
      .MapGet(string.Empty, async ([AsParameters] GetWholeUsersQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      );

    group
      .MapGet("/apps", async ([AsParameters] GetUsersQuery query, ISender sender) =>
        await sender.Send(query).AsHttpResult()
      );

    group
      .MapPost(string.Empty, async (AddUserCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      )
      .Produces<CreatedUserDto>();

    group
      .MapPost("/credentials", async (AddCredentialsCommand cmd, ISender sender) =>
        await sender.Send(cmd).AsHttpResult()
      );
  }
}
