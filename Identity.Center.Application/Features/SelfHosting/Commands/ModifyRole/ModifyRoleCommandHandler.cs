using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Domain.Entities.Core.Identity;
using Action =  Identity.Center.Domain.Entities.Core.Security.Action;
using Microsoft.Extensions.DependencyInjection;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;
using Identity.Center.Domain.Enums;
using System.Text.Json;
using System.Net;
using Identity.Center.Domain.Common;

namespace Identity.Center.Application.Features.SelfHosting.Commands.ModifyRole;

internal sealed class ModifyRoleCommandHandler(IServiceProvider provider) : ICommandHandler<ModifyRoleCommand, ModifiedUserDto>
{
  private readonly IIdentityRepository<Role> _roleRepo = provider.GetRequiredService<IIdentityRepository<Role>>();
  private readonly IIdentityRepository<ChangeControl> _changeRepo = provider.GetRequiredService<IIdentityRepository<ChangeControl>>();
  private readonly IIdentityRepository<RoleClaim> _roleClaimRepo = provider.GetRequiredService<IIdentityRepository<RoleClaim>>();
  private readonly IIdentityRepository<ClaimValue> _claimsRepo = provider.GetRequiredService<IIdentityRepository<ClaimValue>>();
  private readonly IIdentityRepository<Action> _actionRepo = provider.GetRequiredService<IIdentityRepository<Action>>();
  private readonly IIdentityRepository<Group> _goupRepo = provider.GetRequiredService<IIdentityRepository<Group>>();
  private readonly IIdentityUnitOfWork _unitOfWork = provider.GetRequiredService<IIdentityUnitOfWork>();

  public async Task<Result<ModifiedUserDto>> Handle(ModifyRoleCommand request, CancellationToken cancellationToken)
  {
    ChangeControl control = await _changeRepo.Data
      .FirstAsync(row => row.Id == request.ChangeControlId, cancellationToken);

    control.Status = request.Status;
    _changeRepo.Update(control);
    
    if (request.Status == ChangeControlStates.Approved)
    {
      RolePictureDto? picture = control.RequestPicture.Deserialize<RolePictureDto>(JsonSerializerOptions.Web);

      if (picture == null)
        return Result.Result.Failure<ModifiedUserDto>(
          HttpStatusCode.NotFound,
          new BaseError("Role.NotFound", "No se encontró la referencia del cambio")
        );

      Role role = await _roleRepo.Data.FirstAsync(row => row.Id == control.RoleId, cancellationToken);
      role.Name = picture.Name ?? role.Name;
      role.Description = picture.Description ?? role.Description;
      role.DomainName = picture.DomainName ?? role.DomainName;
      role.ActiveDirectoryMandatory = picture.ActiveDiretoryMandatory ?? role.ActiveDirectoryMandatory;
      role.Root = picture.Root ?? role.Root;
      _roleRepo.Update(role);
      await _unitOfWork.SaveChangesAsync(cancellationToken);
      if (picture.Claims != null)
      {
        IEnumerable<string> actualClaims = await _roleClaimRepo.Data
          .Include(row => row.Claim.Group)
          .Include(row => row.Claim.Action)
          .Where(row => row.RoleId == control.RoleId)
          .Select(row => row.Claim.Group.Name + ":" + row.Claim.Action.Name)
          .ToListAsync(cancellationToken);

        IEnumerable<string> newClaims = picture.Claims.Where(c => !actualClaims.Contains(c));
        IEnumerable<string> remotionClaims = actualClaims.Where(c => !picture.Claims.Contains(c));

        foreach (string remotion in remotionClaims)
        {
          RoleClaim roleClaim = await _roleClaimRepo.Data
            .FirstAsync(row => row.RoleId == control.RoleId && row.Claim.Group.Name + ":" + row.Claim.Action.Name == remotion, cancellationToken);

          _roleClaimRepo.Remove(roleClaim);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (string add in newClaims)
        {
          ClaimValue? claim = await _claimsRepo.Data
            .FirstOrDefaultAsync(row => row.Group.Name + ":" + row.Action.Name == add, cancellationToken);

          if (claim == null)
          {
            KeyValuePair<string, string> pairs = IdentityCommons.Deserialize(add);
            Guid groupId = await _goupRepo.Data
              .Where(row => row.Name == pairs.Key)
              .Select(row => row.Id)
              .FirstAsync(cancellationToken);

            Guid actionId = await _actionRepo.Data
              .Where(row => row.Name == pairs.Value)
              .Select(row => row.Id)
              .FirstAsync(cancellationToken);

            claim = new()
            {
              ActionId = actionId,
              GroupId = groupId,
            };

            await _claimsRepo.AddAsync(claim, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
          }

          RoleClaim roleClaim = new()
          {
            ClaimId = claim.Id,
            RoleId = control.RoleId
          };

          await _roleClaimRepo.AddAsync(roleClaim, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
      }
    }

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ModifiedUserDto(control.Id);
  }
}
