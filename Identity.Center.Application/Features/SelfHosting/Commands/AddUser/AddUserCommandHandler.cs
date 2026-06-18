using System.Net;
using System.Security.Claims;
using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Common.Models.Cryptography;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.SelfHosting.Commands.AddUser;

internal sealed class AddUserCommandHandler(IServiceProvider provider) : ICommandHandler<AddUserCommand, CreatedUserDto>
{
  private readonly IIdentityUnitOfWork _unitOfWork = provider.GetRequiredService<IIdentityUnitOfWork>();
  private readonly IIdentityRepository<User> _userRepo = provider.GetRequiredService<IIdentityRepository<User>>();
  private readonly IIdentityRepository<UserRole> _userRoleRepo = provider.GetRequiredService<IIdentityRepository<UserRole>>();
  private readonly IIdentityRepository<ContactType> _contactTypeRepo = provider.GetRequiredService<IIdentityRepository<ContactType>>();
  private readonly IIdentityRepository<CredentialType> _credentialTypeRepo = provider.GetRequiredService<IIdentityRepository<CredentialType>>();
  private readonly IIdentityRepository<DomainCredential> _domainRepo = provider.GetRequiredService<IIdentityRepository<DomainCredential>>();
  private readonly IIdentityRepository<SingleCredential> _singleRepo = provider.GetRequiredService<IIdentityRepository<SingleCredential>>();
  private readonly IIdentityRepository<ContactInfo> _contactRepo = provider.GetRequiredService<IIdentityRepository<ContactInfo>>();
  private readonly IHttpContextAccessor _context = provider.GetRequiredService<IHttpContextAccessor>();

  public async Task<Result<CreatedUserDto>> Handle(AddUserCommand request, CancellationToken cancellationToken)
  {
    if (
      _context.HttpContext == null ||
      _context.HttpContext.User.FindFirstValue(IdentityClaimTypes.App) is not string appClaim ||
      !Guid.TryParse(appClaim, out Guid appId)
    )
      return Result.Result.Failure<CreatedUserDto>(
        HttpStatusCode.Unauthorized,
        new BaseError("Invalid.Token", "Token invalido")
      );

    Guid? userId = request.UserRefId;

    if (!userId.HasValue)
    {
      AddUserDto dto = request.UserInfo!;
      User newUser = new()
      {
        DocumentNumber = dto.DocumentNumber,
        DocumentType = dto.DocumentType,
        FirstLastName = dto.FirstLastName,
        FirstName = dto.FirstName,
        SecondName = dto.SecondName,
        SecondLastName = dto.SecondLastName,
      };

      await _userRepo.AddAsync(newUser, cancellationToken);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      Dictionary<ContactTypes, Guid> contactTypePairs = await _contactTypeRepo.Data
        .ToDictionaryAsync(
          row => row.ContactTypeKey,
          row => row.Id,
          cancellationToken
        );

      Dictionary<int, AuthenticationMethods> credentialTypePairs = await _credentialTypeRepo.Data
        .ToDictionaryAsync(
          row => row.Id,
          row => row.AuthType,
          cancellationToken
        );

      await _contactRepo.AddRangeAsync(
        dto.ContacInfo.Select(row => new ContactInfo
        {
          ContactTypeId = contactTypePairs[row.Type],
          UserId = newUser.Id,
          Value = row.Value,
          Salt = IdentityCommons.NewHashKey
        }),
        cancellationToken
      );

      if (dto.Credencials.Any(c => credentialTypePairs[c.CredentialType] == AuthenticationMethods.Quamtum))
        await _domainRepo.AddRangeAsync(
          dto.Credencials
            .Where(c => credentialTypePairs[c.CredentialType] == AuthenticationMethods.Quamtum)
            .Select(row => new DomainCredential
            {
              CredentialTypeId = row.CredentialType,
              UserId = newUser.Id,
              Username = row.Username
            }),
          cancellationToken
        );

      if (dto.Credencials.Any(c => credentialTypePairs[c.CredentialType] == AuthenticationMethods.Single))
        await _singleRepo.AddRangeAsync(
          await Task.WhenAll(
            dto.Credencials
              .Where(c => credentialTypePairs[c.CredentialType] == AuthenticationMethods.Single)
              .Select(async row =>
              {
                HashCreationResponse response = await IdentityCommons.NewHash();
                return new SingleCredential
                {
                  AppId = appId,
                  UserId = newUser.Id,
                  Username = row.Username,
                  Hash = response.Hash,
                  Salt = response.Salt
                };
              })
          ),
          cancellationToken
        );

      await _unitOfWork.SaveChangesAsync(cancellationToken);

      userId = newUser.Id;
    }

    await _userRoleRepo.AddRangeAsync(
      request.Roles.Select(row => new UserRole
      {
        RoleId = row,
        UserId = userId.Value,
      }),
      cancellationToken
    );

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new CreatedUserDto(userId.Value);
  }
}
