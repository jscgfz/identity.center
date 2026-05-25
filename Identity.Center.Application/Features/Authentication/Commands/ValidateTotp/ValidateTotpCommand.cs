using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Authentication.Dtos;

namespace Identity.Center.Application.Features.Authentication.Commands.ValidateTotp;

public sealed record ValidateTotpCommand(
  string Totp
) : ICommand<AuthenticationReponseDto>;
