using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Roles.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Action = Identity.Center.Domain.Entities.Core.Security.Action;


namespace Identity.Center.Application.Features.Roles.Commands.AddRole;

internal sealed class AddRoleCommandHandler(IServiceProvider provider) : ICommandHandler<AddRoleCommand, CreatedRoleDto>
{
  private readonly IIdentityRepository<Role> _role = provider.GetRequiredService<IIdentityRepository<Role>>();
  private readonly IIdentityRepository<RoleClaim> _roleClaim = provider.GetRequiredService<IIdentityRepository<RoleClaim>>();
  private readonly IIdentityRepository<ClaimValue> _claim = provider.GetRequiredService<IIdentityRepository<ClaimValue>>();
  private readonly IIdentityRepository<Group> _groupRepo = provider.GetRequiredService<IIdentityRepository<Group>>();
  private readonly IIdentityRepository<Action> _actionRepo = provider.GetRequiredService<IIdentityRepository<Action>>();
  private readonly IIdentityUnitOfWork _unitOfWork = provider.GetRequiredService<IIdentityUnitOfWork>();

  public async Task<Result<CreatedRoleDto>> Handle(AddRoleCommand request, CancellationToken cancellationToken)
  {
    Role role = new()
    {
      AppId = request.AppId,
      Name = request.Name,
      Description = request.Description,
      DomainName = request.DomainName,
      ActiveDirectoryMandatory = request.ActiveDirectoryMandatory,
      Root = request.Root
    };

    await _role.AddAsync(role, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    foreach (string claim in request.Claims)
    {
      ClaimValue? claimValue = await _claim.Data
        .FirstOrDefaultAsync(row => row.Group.Name + ":" + row.Action.Name == claim, cancellationToken);

      if (claimValue == null)
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
        await _claim.AddAsync(claimValue, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
      }

      RoleClaim roleClaim = new()
      {
        ClaimId = claimValue.Id,
        RoleId = role.Id
      };

      await _roleClaim.AddAsync(roleClaim, cancellationToken);
      await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    return new CreatedRoleDto(role.Id);
  }
}
