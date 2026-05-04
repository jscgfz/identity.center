using Identity.Center.Application.Abstractions.Reponses;

namespace Identity.Center.Application.Features.ApiKeys.Dtos;

public sealed record CreatedApkiKeyDto(
  Guid SubjectId,
  string ApiKey
) : ICreatedResponse;
