namespace Identity.Center.Application.Features.Credentials.Dtos;
public sealed record CredentialRequestDto(
  int CredentialTypeId,
  Guid? AppId,
  string Value
);
