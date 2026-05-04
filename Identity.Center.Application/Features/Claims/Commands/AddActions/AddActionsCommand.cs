using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Claims.Dtos;

namespace Identity.Center.Application.Features.Claims.Commands.AddActions;

public sealed record AddActionsCommand(
  IEnumerable<CreateClaimPartDto> Cmd
) : ICommand<CreatedClaimPartsDto>;
