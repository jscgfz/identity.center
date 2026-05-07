using Identity.Center.Application.Abstractions.Reponses;

namespace Identity.Center.Application.Features.Credentials.Dtos;

public sealed class CreatedCredentialsDto : List<CreatedCredentialDto>, ICreatedResponse
{ }

public sealed record CreatedCredentialDto(
  Guid? Id,
  Guid? AppId,
  int CredentialTypeId,
  string Value
);