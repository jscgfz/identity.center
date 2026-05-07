using System.Threading;
using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Application.Features.Credentials.Commands.AddCredentials;

internal sealed class AddCredentialsCommandValidator : AbstractValidator<AddCredentialsCommand>
{
  public AddCredentialsCommandValidator(
    IIdentityRepository<User> userRepo,
    IIdentityRepository<SingleCredential> singleCredentialRepo,
    IIdentityRepository<DomainCredential> domainCredentialRepo,
    IIdentityRepository<CredentialType> typeRepo
  )
  {
    RuleFor(row => row.UserId)
      .MustAsync(async (field, cancellationToken) => await userRepo.Data.AnyAsync(row => row.Id == field, cancellationToken))
      .WithName("User")
      .WithErrorCode("NotFound")
      .WithMessage("No se encontró ningún usuario con el identificador");

    RuleForEach(row => row.Credentials)
      .MustAsync(async (field, cancellationToken) => await typeRepo.Data.AnyAsync(row => row.Id == field.CredentialTypeId, cancellationToken))
      .WithName("CredentielType")
      .WithErrorCode("NotFound")
      .WithMessage("No se encontro tipo de credencial con el identificador");

    RuleForEach(row => row.Credentials)
      .WhereAsync(async (field) => await typeRepo.Data.AnyAsync(row => row.Id == field.CredentialTypeId && row.AuthType == Domain.Enums.AuthenticationMethods.Single))
      .Must(row => row.AppId.HasValue)
      .WithErrorCode("Invalid")
      .WithMessage("Las credenciales simples deben ir relacionanadas con identificador de aplicación")
      .MustAsync(async (field, cancellationToken) => !await singleCredentialRepo.Data.AnyAsync(row => row.Username == field.Value && row.AppId == field.AppId, cancellationToken))
      .WithErrorCode("Duplicated")
      .WithMessage((ob, field) => $"Credenciales con nombre de usuario {field.Value} ya registradas");

    RuleForEach(row => row.Credentials)
      .WhereAsync(async (field) => await typeRepo.Data.AnyAsync(row => row.Id == field.CredentialTypeId && row.AuthType == Domain.Enums.AuthenticationMethods.Quamtum))
      .MustAsync(async (field, cancellationToken) => !await domainCredentialRepo.Data.AnyAsync(row => row.Username == field.Value && row.CredentialTypeId == field.CredentialTypeId, cancellationToken))
      .WithErrorCode("Duplicated")
      .WithMessage((ob, field) => $"Credenciales con nombre de usuario {field.Value} ya registradas");
  }
}
