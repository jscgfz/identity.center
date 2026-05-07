using Identity.Center.Application.Abstractions.Reponses;

namespace Identity.Center.Application.Features.Users.Dtos;

public sealed record CreatedUserDto(
  Guid Id
) : ICreatedResponse;
