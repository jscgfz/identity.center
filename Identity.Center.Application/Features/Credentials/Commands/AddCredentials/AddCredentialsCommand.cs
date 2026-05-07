using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Credentials.Dtos;

namespace Identity.Center.Application.Features.Credentials.Commands.AddCredentials;

public sealed record AddCredentialsCommand(
  Guid UserId,
  IEnumerable<CredentialRequestDto> Credentials
) : ICommand<CreatedCredentialsDto>;
