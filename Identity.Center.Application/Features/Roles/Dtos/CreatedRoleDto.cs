using Identity.Center.Application.Abstractions.Reponses;

namespace Identity.Center.Application.Features.Roles.Dtos;

public sealed record CreatedRoleDto(
  Guid Id
) : ICreatedResponse;
