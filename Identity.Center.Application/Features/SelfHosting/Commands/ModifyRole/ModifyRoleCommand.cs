using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Identity.Center.Domain.Enums;

namespace Identity.Center.Application.Features.SelfHosting.Commands.ModifyRole;

public sealed record ModifyRoleCommand(
  Guid ChangeControlId,
  ChangeControlStates Status
) : ICommand<ModifiedUserDto>;
