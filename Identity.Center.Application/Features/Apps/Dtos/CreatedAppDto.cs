using Identity.Center.Application.Abstractions.Reponses;

namespace Identity.Center.Application.Features.Apps.Dtos;

public sealed record CreatedAppDto(
  Guid Id
) : ICreatedResponse;
