using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Authentication.Dtos;
using MediatR;

namespace Identity.Center.Application.Features.Authentication.Commands.ValidateTotp;

public sealed record ValidateTotpCommand(
  string Totp
) : ICommand<AuthenticationReponseDto>;
