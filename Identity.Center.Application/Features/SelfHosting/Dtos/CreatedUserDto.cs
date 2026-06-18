using Identity.Center.Application.Abstractions.Reponses;

namespace Identity.Center.Application.Features.SelfHosting.Dtos;
public sealed record CreatedUserDto(
  Guid Id
) : ICreatedResponse;
