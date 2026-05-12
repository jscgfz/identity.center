using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Roles.Dtos;

namespace Identity.Center.Application.Features.Roles.Commands.AddRole;
public sealed record AddRoleCommand(
  Guid AppId,
  string Name,
  string Description,
  string? DomainName,
  bool ActiveDirectoryMandatory,
  bool Root,
  IEnumerable<string> Claims
) : ICommand<CreatedRoleDto>;
