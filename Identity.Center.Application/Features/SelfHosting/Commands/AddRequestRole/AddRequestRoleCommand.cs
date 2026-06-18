using System.Text.Json.Serialization;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Microsoft.AspNetCore.Http;

namespace Identity.Center.Application.Features.SelfHosting.Commands.AddRequestRole;

public sealed record AddRequestRoleCommand(
  Guid RoleId,
  [property: JsonPropertyName("info")] ModifyRoleRequestDto Dto
) : ICommand<ModifiedUserDto>;

public sealed record ModifyRoleRequestDto(
  string Reason,
  IFormFile AuthorizationFile,
  string? Name = null,
  string? Description = null,
  string? DomainName = null,
  bool? ActiveDiretoryMandatory = null,
  bool? Root = null,
  IEnumerable<string>? Claims = null
);