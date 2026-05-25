using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Users.Dtos;

namespace Identity.Center.Application.Features.Users.Commands.AddUser;

public sealed record AddUserCommand(
  string DocumentType,
  string DocumentNumber,
  string FirstName,
  string? SecondName,
  string FirstLastName,
  string? SecondLastName,
  IEnumerable<ContactInfoRequestDto> ContactInfo
) : ICommand<CreatedUserDto>;
