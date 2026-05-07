using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Users.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Users.Commands.AddUser;

internal sealed class AddUserCommandHandler(IServiceProvider provider) : ICommandHandler<AddUserCommand, CreatedUserDto>
{
  private readonly IIdentityRepository<User> _userRepo = provider.GetRequiredService<IIdentityRepository<User>>();
  private readonly IIdentityRepository<ContactInfo> _contactRepo = provider.GetRequiredService<IIdentityRepository<ContactInfo>>();
  private readonly IIdentityRepository<ContactType> _typeRepo = provider.GetRequiredService<IIdentityRepository<ContactType>>();
  private readonly IIdentityUnitOfWork _unitOfWork = provider.GetRequiredService<IIdentityUnitOfWork>();

  public async Task<Result<CreatedUserDto>> Handle(AddUserCommand request, CancellationToken cancellationToken)
  {
    User user = new()
    {
      DocumentType = request.DocumentType,
      DocumentNumber = request.DocumentNumber,
      FirstName = request.FirstName,
      SecondName = request.SecondName,
      FirstLastName = request.FirstLastName,
      SecondLastName = request.SecondLastName,
    };

    await _userRepo.AddAsync(user, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    IEnumerable<ContactTypes> enumTypes = request.ContactInfo.Select(row => row.ContactTypeId).Distinct();

    IEnumerable<ContactType> types = await _typeRepo.Data
      .AsNoTracking()
      .Where(row => enumTypes.Contains(row.ContactTypeKey))
      .ToListAsync(cancellationToken);

    IEnumerable<ContactInfo> contactInfo = request.ContactInfo
      .Select(ci => new ContactInfo()
      {
        ContactTypeId = types.First(t => t.ContactTypeKey == ci.ContactTypeId).Id,
        Value = ci.Value,
        Salt = IdentityCommons.NewHashKey,
        UserId = user.Id
      });

    await _contactRepo.AddRangeAsync(contactInfo, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new CreatedUserDto(
      user.Id
    );
  }
}
