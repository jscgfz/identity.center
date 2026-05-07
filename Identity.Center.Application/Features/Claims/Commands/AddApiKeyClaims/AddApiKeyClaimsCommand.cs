using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Claims.Dtos;

namespace Identity.Center.Application.Features.Claims.Commands.AddApiKeyClaims;

public sealed record AddApiKeyClaimsCommand(
  Guid SubjectId,
  IEnumerable<string> Claims
) : ICommand<RelatedClaimDto>;
