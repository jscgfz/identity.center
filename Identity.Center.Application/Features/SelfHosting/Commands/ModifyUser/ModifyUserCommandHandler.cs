using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.SelfHosting.Commands.ModifyUser;

internal sealed class ModifyUserCommandHandler(IServiceProvider provider) : ICommandHandler<ModifyUserCommand, ModifiedUserDto>
{
  private readonly IIdentityRepository<User> _userRepo = provider.GetRequiredService<IIdentityRepository<User>>();
  private readonly IIdentityUnitOfWork _unitOfWork = provider.GetRequiredService<IIdentityUnitOfWork>();

  public async Task<Result<ModifiedUserDto>> Handle(ModifyUserCommand request, CancellationToken cancellationToken)
  {
    User user = await _userRepo.Data.FirstAsync(row => row.Id == request.UserId, cancellationToken);
    user.DocumentType = request.Dto.DocumentType ?? user.DocumentType;
    user.DocumentNumber = request.Dto.DocumentNumber ?? user.DocumentNumber;
    user.FirstName = request.Dto.FirstName ?? user.FirstName;
    user.SecondName = request.Dto.SecondName ?? user.SecondName;
    user.FirstLastName = request.Dto.FirstLastName ?? user.FirstLastName;
    user.SecondLastName = request.Dto.SecondLastName ?? user.SecondLastName;
    _userRepo.Update(user);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    return new ModifiedUserDto(user.Id);
  }
}
