using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Claims.Dtos;
using Identity.Center.Application.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Action = Identity.Center.Domain.Entities.Core.Security.Action;

namespace Identity.Center.Application.Features.Claims.Commands.AddActions;

internal sealed class AddActionsCommandHandler(IServiceProvider provider) : ICommandHandler<AddActionsCommand, CreatedClaimPartsDto>
{
  private readonly IIdentityRepository<Action> _repo = provider.GetRequiredService<IIdentityRepository<Action>>();
  private readonly IIdentityUnitOfWork _unitOfWork = provider.GetRequiredService<IIdentityUnitOfWork>();

  public async Task<Result<CreatedClaimPartsDto>> Handle(AddActionsCommand request, CancellationToken cancellationToken)
  {
    IEnumerable<Action> actions = [];
    foreach(CreateClaimPartDto part in request.Cmd)
    {
      Action? currentAction = await _repo.Data
        .FirstOrDefaultAsync(row => row.Name == part.Name, cancellationToken);

      if(currentAction == null)
      {
        currentAction = new()
        {
          Name = part.Name,
          Description = part.Description
        };

        await _repo.AddAsync(currentAction, cancellationToken);
      }
      else
      {
        currentAction.Description ??= part.Description;
        _repo.Update(currentAction);
      }

      actions = actions.Append(currentAction);
    }
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    CreatedClaimPartsDto result = [];
    foreach (Action action in actions)
      result.Add(
        new(
          action.Id,
          action.Name,
          action.Description,
          action.CreatedAtUtc
        )
      );

    return result;
  }
}
