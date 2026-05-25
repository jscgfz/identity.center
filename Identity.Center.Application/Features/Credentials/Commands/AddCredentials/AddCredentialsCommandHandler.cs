using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Credentials.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Common.Models.Cryptography;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Credentials.Commands.AddCredentials;
internal sealed class AddCredentialsCommandHandler(IServiceProvider provider) : ICommandHandler<AddCredentialsCommand, CreatedCredentialsDto>
{
  private readonly IIdentityRepository<CredentialType> _typeRepo = provider.GetRequiredService<IIdentityRepository<CredentialType>>();
  private readonly IIdentityRepository<DomainCredential> _domainRepo = provider.GetRequiredService<IIdentityRepository<DomainCredential>>();
  private readonly IIdentityRepository<SingleCredential> _singleRepo = provider.GetRequiredService<IIdentityRepository<SingleCredential>>();
  private readonly IIdentityUnitOfWork _unitOfWork = provider.GetRequiredService<IIdentityUnitOfWork>();

  public async Task<Result<CreatedCredentialsDto>> Handle(AddCredentialsCommand request, CancellationToken cancellationToken)
  {
    CreatedCredentialsDto createdCredentialDtos = [];
    IEnumerable<int> typeIds = request.Credentials.Select(row => row.CredentialTypeId).Distinct();
    IEnumerable<KeyValuePair<AuthenticationMethods, CredentialRequestDto>> credentials = request
      .Credentials
      .Join(
        await _typeRepo.Data
          .Where(row => typeIds.Contains(row.Id))
          .Select(row => KeyValuePair.Create(row.Id, row.AuthType))
          .ToListAsync(cancellationToken),
        c => c.CredentialTypeId,
        t => t.Key,
        (c, t) => KeyValuePair.Create(t.Value, c)
      );

    foreach (CredentialRequestDto credential in credentials.Where(row => row.Key == AuthenticationMethods.Single).Select(row => row.Value))
    {
      HashCreationResponse hash = await IdentityCommons.NewHash(null);
      SingleCredential singleCredential = new()
      {
        AppId = credential.AppId!.Value,
        Hash = hash.Hash,
        Salt = hash.Salt,
        UserId = request.UserId,
        Username = credential.Value
      };

      await _singleRepo.AddAsync(singleCredential, cancellationToken);
      await _unitOfWork.SaveChangesAsync(cancellationToken);
      createdCredentialDtos.Add(new(
        null,
        singleCredential.AppId,
        credential.CredentialTypeId,
        singleCredential.Username,
        singleCredential.CreatedAtUtc
      ));
    }

    foreach (CredentialRequestDto credential in credentials.Where(row => row.Key == AuthenticationMethods.Quamtum).Select(row => row.Value))
    {
      DomainCredential domainCredential = new()
      {
        CredentialTypeId = credential.CredentialTypeId,
        UserId = request.UserId,
        Username = credential.Value
      };
      await _domainRepo.AddAsync(domainCredential, cancellationToken);
      await _unitOfWork.SaveChangesAsync(cancellationToken);
      createdCredentialDtos.Add(new(
        domainCredential.Id,
        null,
        domainCredential.CredentialTypeId,
        domainCredential.Username,
        domainCredential.CreatedAtUtc
      ));
    }

    return createdCredentialDtos;
  }
}
