using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Claims.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Action = Identity.Center.Domain.Entities.Core.Security.Action;

namespace Identity.Center.Application.Features.Claims.Commands.AddApiKeyClaims;

internal sealed class AddApiKeyClaimsCommandHandler(IServiceProvider provider) : ICommandHandler<AddApiKeyClaimsCommand, RelatedClaimDto>
{
  private readonly IIdentityRepository<ApiKeyClaim> _apikeyRepo = provider.GetRequiredService<IIdentityRepository<ApiKeyClaim>>();
  private readonly IIdentityRepository<ClaimValue> _claimRepo = provider.GetRequiredService<IIdentityRepository<ClaimValue>>();
  private readonly IIdentityRepository<Group> _groupRepo = provider.GetRequiredService<IIdentityRepository<Group>>();
  private readonly IIdentityRepository<Action> _actionRepo = provider.GetRequiredService<IIdentityRepository<Action>>();
  private readonly IIdentityUnitOfWork _unitOfWork = provider.GetRequiredService<IIdentityUnitOfWork>();

  public async Task<Result<RelatedClaimDto>> Handle(AddApiKeyClaimsCommand request, CancellationToken cancellationToken)
  {
    RelatedClaimDto claims = [];
    foreach (string claim in request.Claims)
    {
      ClaimValue? claimValue = await _claimRepo.Data
        .FirstOrDefaultAsync(row => (row.Group.Name + ":" + row.Action.Name) == claim, cancellationToken);

      if (claimValue is null)
      {
        KeyValuePair<string, string> pairs = IdentityCommons.Deserialize(claim);
        Guid groupId = await _groupRepo.Data
          .Where(row => row.Name == pairs.Key)
          .Select(row => row.Id)
          .FirstAsync(cancellationToken);
        Guid actionId = await _actionRepo.Data
          .Where(row => row.Name == pairs.Value)
          .Select(row => row.Id)
          .FirstAsync(cancellationToken);
        claimValue = new()
        {
          ActionId = actionId,
          GroupId = groupId,
        };
        await _claimRepo.AddAsync(claimValue, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
      }

      if (!await _apikeyRepo.Data.AnyAsync(row => row.ApiKeyId == request.SubjectId && row.ClaimId == claimValue.Id, cancellationToken))
      {
        ApiKeyClaim apiKeyClaim = new()
        {
          ApiKeyId = request.SubjectId,
          ClaimId = claimValue.Id,
        };

        await _apikeyRepo.AddAsync(apiKeyClaim, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
      }

      claims.Add(claim);
    }

    return claims;
  }
}
