using System.Text.Json.Serialization;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.SelfHosting.Dtos;

namespace Identity.Center.Application.Features.SelfHosting.Commands.ModifyUser;

public sealed record ModifyUserCommand(
  Guid UserId,
  [property: JsonPropertyName("info")] UserModificationDto Dto
) : ICommand<ModifiedUserDto>;


public sealed record UserModificationDto(
  string? DocumentType,
  string? DocumentNumber,
  string? FirstName,
  string? FirstLastName,
  string? SecondName,
  string? SecondLastName
);