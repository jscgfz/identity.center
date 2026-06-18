using System.Windows.Input;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Identity.Center.Domain.Enums;

namespace Identity.Center.Application.Features.SelfHosting.Commands.AddUser;

public sealed record AddUserCommand(
  Guid? UserRefId,
  AddUserDto? UserInfo,
  IEnumerable<Guid> Roles
) : ICommand<CreatedUserDto>;

public sealed record AddUserDto(
  string DocumentType,
  string DocumentNumber,
  string FirstName,
  string? SecondName,
  string FirstLastName,
  string? SecondLastName,
  IEnumerable<AddContactInfoDto> ContacInfo,
  IEnumerable<AddCredentialDto> Credencials
);


public sealed record AddContactInfoDto(
  ContactTypes Type,
  string Value
);

public sealed record AddCredentialDto(
  int CredentialType,
  string Username
);