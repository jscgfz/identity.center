using System.Text.Json.Serialization;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Enums;
using Microsoft.AspNetCore.Routing;

namespace Identity.Center.Application.Features.SelfHosting.Dtos;

public sealed record OwnUserDto(
  Guid Id,
  string DocumentType,
  string DocumentNumber,
  string FirstName,
  string? SecondName,
  string FirstLastName,
  string? SecondLastName,
  DateTimeOffset CratedAtUtc,
  IEnumerable<OwnContactInfoDto> ContactInfo,
  IEnumerable<Guid> Roles,
  [property: JsonIgnore] IEnumerable<SingleCredential> SingleCredentials,
  [property: JsonIgnore] IEnumerable<DomainCredential> DomainCredentials
)
{
  public IEnumerable<OwnCredentialDto> Credentials => [
    .. SingleCredentials.Select(row => new OwnCredentialDto(
      null,
      row.AppId,
      row.Username,
      new(
        null,
        AuthenticationMethods.Single
      )
    )),
    .. DomainCredentials.Select(row => new OwnCredentialDto(
      row.Id,
      null,
      row.Username,
      new(
        row.CredentialType.Id,
        row.CredentialType.AuthType
      )
    ))
  ];
}


public sealed record OwnContactInfoDto(
  ContactTypes Type,
  string Value,
  bool Confirmed,
  DateTimeOffset CratedAtUtc
);

public sealed record OwnCredentialDto(
  Guid? Id,
  Guid? AppId,
  string Username,
  OwnCredentialTypeDto Type
);

public sealed record OwnCredentialTypeDto(
  int? Id,
  AuthenticationMethods Method
);