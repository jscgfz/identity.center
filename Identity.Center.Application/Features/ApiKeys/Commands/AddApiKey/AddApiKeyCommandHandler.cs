using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.ApiKeys.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Common.Models.Cryptography;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Action = Identity.Center.Domain.Entities.Core.Security.Action;

namespace Identity.Center.Application.Features.ApiKeys.Commands.AddApiKey;

internal sealed class AddApiKeyCommandHandler(IServiceProvider provider) : ICommandHandler<AddApiKeyCommand, CreatedApkiKeyDto>
{
  private readonly IIdentityRepository<ApiKey> _apikeyRepo = provider.GetRequiredService<IIdentityRepository<ApiKey>>();
  private readonly IIdentityRepository<ApiKeyClaim> _claimsRepo = provider.GetRequiredService<IIdentityRepository<ApiKeyClaim>>();
  private readonly IIdentityRepository<ClaimValue> _valuesRepo = provider.GetRequiredService<IIdentityRepository<ClaimValue>>();
  private readonly IIdentityRepository<Group> _groupRepo = provider.GetRequiredService<IIdentityRepository<Group>>();
  private readonly IIdentityRepository<Action> _actionsRepo = provider.GetRequiredService<IIdentityRepository<Action>>();
  private readonly IIdentityRepository<App> _appRepo = provider.GetRequiredService<IIdentityRepository<App>>();
  private readonly IIdentityUnitOfWork _unitOfWork = provider.GetRequiredService<IIdentityUnitOfWork>();

  public async Task<Result<CreatedApkiKeyDto>> Handle(AddApiKeyCommand request, CancellationToken cancellationToken)
  {
    App app = await _appRepo.Data.FirstAsync(row => row.Id == request.AppId, cancellationToken);
    HashCreationResponse hash = await IdentityCommons.NewHash(
      $"{app.Prefix}-{Convert.ToHexString(IdentityCommons.NewHashKey).ToLower()}",
      cancellationToken
    );
    ApiKey apikey = new()
    {
      AppId = request.AppId,
      Name = request.Name,
      Hash = hash.Hash,
      Salt = hash.Salt,
      Description = request.Description,
      Root = request.Root
    };
    await _apikeyRepo.AddAsync(apikey, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    foreach (string claim in request.Claims)
    {
      ClaimValue? claimValue = await _valuesRepo.Data
        .FirstOrDefaultAsync(row => row.Group.Name + ":" + row.Action.Name == claim, cancellationToken);

      if (claimValue == null)
      {
        KeyValuePair<string, string> parts = IdentityCommons.Deserialize(claim);
        Guid actionId = await _actionsRepo.Data
          .Where(row => row.Name == parts.Value)
          .Select(row => row.Id)
          .FirstAsync(cancellationToken);

        Guid groupId = await _groupRepo.Data
          .Where(row => row.Name == parts.Key)
          .Select(row => row.Id)
          .FirstAsync(cancellationToken);

        claimValue = new()
        {
          ActionId = actionId,
          GroupId = groupId
        };

        await _valuesRepo.AddAsync(claimValue, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
      }

      ApiKeyClaim apiKeyClaim = new()
      {
        ApiKeyId = apikey.Id,
        ClaimId = claimValue.Id
      };

      await _claimsRepo.AddAsync(apiKeyClaim, cancellationToken);
      await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    return new CreatedApkiKeyDto(
      apikey.Id,
      hash.Value
    );
  }
}
