using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Claims.Dtos;

namespace Identity.Center.Application.Features.Claims.Commands.AddGroups;

public sealed record AddGroupsCommand(
  IEnumerable<CreateClaimPartDto> Cmd
) : ICommand<CreatedClaimPartsDto>;
