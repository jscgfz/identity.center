using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Claims.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Claims.Commands.AddGroups;

internal sealed class AddGroupsCommandHandler(IServiceProvider provider) : ICommandHandler<AddGroupsCommand, CreatedClaimPartsDto>
{
  private readonly IIdentityRepository<Group> _repo = provider.GetRequiredService<IIdentityRepository<Group>>();
  private readonly IIdentityUnitOfWork _unitOfWork = provider.GetRequiredService<IIdentityUnitOfWork>();

  public async Task<Result<CreatedClaimPartsDto>> Handle(AddGroupsCommand request, CancellationToken cancellationToken)
  {
    IEnumerable<Group> groups = [];
    foreach (CreateClaimPartDto part in request.Cmd)
    {
      Group? currentGroup = await _repo.Data
        .FirstOrDefaultAsync(row => row.Name == part.Name, cancellationToken);

      if (currentGroup == null)
      {
        currentGroup = new()
        {
          Name = part.Name,
          Description = part.Description
        };

        await _repo.AddAsync(currentGroup, cancellationToken);
      }
      else
      {
        currentGroup.Description ??= part.Description;
        _repo.Update(currentGroup);
      }

      groups = groups.Append(currentGroup);
    }
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    CreatedClaimPartsDto result = [];
    foreach (Group group in groups)
      result.Add(
        new(
          group.Id,
          group.Name,
          group.Description,
          group.CreatedAtUtc
        )
      );

    return result;
  }
}
