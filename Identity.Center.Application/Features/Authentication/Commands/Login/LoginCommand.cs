using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Authentication.Dtos;

namespace Identity.Center.Application.Features.Authentication.Commands.Login;

public sealed record LoginCommand(
  Guid AppId,
  string Username,
  string Password
) : LoginRequestDto(Username, Password), ICommand<AuthenticationReponseDto>;
