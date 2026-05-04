using Identity.Center.Application.Abstractions.Reponses;

namespace Identity.Center.Application.Features.Claims.Dtos;

public sealed class CreatedClaimPartsDto : List<MasterClaimPart>, ICreatedResponse
{ }
