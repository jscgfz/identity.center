using System.Text.RegularExpressions;
using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Common.Options;
using Identity.Center.Application.Extensions;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Center.Application.Features.SelfHosting.Commands.AddUser;

internal sealed class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
  public AddUserCommandValidator(
    IIdentityRepository<Role> roleRepo,
    IIdentityRepository<User> userRepo,
    IIdentityRepository<CredentialType> typeRepo,
    IIdentityRepository<SingleCredential> singleRepo,
    IIdentityRepository<DomainCredential> domainRepo,
    IIdentityRepository<App> appRepo,
    IHttpContextAccessor context,
    IOptionsMonitor<ContactTypesOptions> options
  )
  {
    RuleForEach(row => row.Roles)
      .MustAsync(async (field, cancellationToken) =>
      {
        Result<Guid> appResult = await context.RetrieveAppContext(cancellationToken);
        return appResult.Success && await roleRepo.Data.AnyAsync(row => row.AppId == appResult.Value && row.Id == field, cancellationToken);
      });

    When(
      row => row.UserRefId.HasValue && row.UserRefId.Value != Guid.Empty,
      () => {
        RuleFor(row => row.UserInfo)
          .NotEmpty()
          .WithName("User")
          .WithMessage("Debe haber alguna referencia de usuario - $.userInfo")
          .ChildRules(child =>
          {
            child.RuleFor(row => row!.DocumentType)
              .NotEmpty()
              .WithMessage("Tipo de documento obligatorio");

            child.RuleFor(row => row!.DocumentNumber)
            .EmailAddress()
              .NotEmpty()
              .WithMessage("Número de documento obligatorio");
            
            child.RuleFor(row => KeyValuePair.Create(row!.DocumentType, row!.DocumentNumber))
              .MustAsync(async (field, cancellationToken) => await userRepo.Data.AnyAsync(row => row.DocumentType == field.Key && row.DocumentNumber == field.Value, cancellationToken))
              .WithMessage((_, pair) => $"Ya existe un usuario con Documento {pair.Key} - {pair.Value}");
            
            child.RuleFor(row => row!.FirstName)
              .NotEmpty()
              .WithMessage("Primer Nombre obligatorio");
            
            child.RuleFor(row => row!.FirstLastName)
              .NotEmpty()
              .WithMessage("Primer Apellido obligatorio");

            child.RuleFor(row => row!.ContacInfo)
              .NotEmpty()
              .WithMessage("Información de contacto obligatoria");

            child.RuleForEach(row => row!.ContacInfo)
              .Where(row => ContactTypes.CorporativeMail == row.Type)
              .Must(field => options.CurrentValue.EmailExpressions.Any(ee => Regex.IsMatch(field.Value, ee)))
              .WithMessage($"Formato invalido => {string.Join(',', options.CurrentValue.EmailExpressions)}");

            child.RuleForEach(row => row!.ContacInfo)
              .Where(row => ContactTypes.Cellphone == row.Type)
              .Must(field => options.CurrentValue.CellPhoneExpressions.Any(ce => Regex.IsMatch(field.Value, ce)))
              .WithMessage($"Formato invalido => {string.Join(',', options.CurrentValue.CellPhoneExpressions)}");

            child.RuleForEach(row => row!.ContacInfo)
              .Where(row => row.Type == ContactTypes.ExternalMail)
              .ChildRules(ci =>
              {
                ci.RuleFor(row => row.Value)
                  .EmailAddress()
                  .WithMessage("Email invalido");
              });

            child.RuleFor(row => row!.Credencials)
              .NotEmpty()
              .WithMessage("Credenciales obligatorios")
              .MustAsync(async (field, cancellationToken) =>
              {
                Result<Guid> app = await context.RetrieveAppContext(cancellationToken);
                IEnumerable<int> types = field.Select(f => f.CredentialType);
                return app.Success && (
                  await typeRepo.Data.AnyAsync(row => types.Contains(row.Id), cancellationToken) ||
                  await appRepo.Data.AnyAsync(row => row.Id == app.Value && row.AllowedCredentials.Any(ac => types.Contains(ac.CredentialTypeId)), cancellationToken)
                );
              })
              .WithErrorCode("Invalid")
              .WithMessage("Debe agreagr al menos una credencial permitida por la aplicación");

            child.RuleForEach(row => row!.Credencials)
              .MustAsync(async (field, cancellationToken) => await typeRepo.Data.AnyAsync(row => row.Id == field.CredentialType, cancellationToken))
              .WithMessage("Tipo de credencial invalido");

            child.RuleForEach(row => row!.Credencials)
              .WhereAsync(async field => await typeRepo.Data.AnyAsync(row => row.Id == field.CredentialType && row.AuthType == AuthenticationMethods.Quamtum))
              .MustAsync(async (field, cancellationToken) => !await domainRepo.Data.AnyAsync(row => row.Username == field.Username, cancellationToken))
              .WithErrorCode("Duplicated")
              .WithMessage((_, row) => $"Ya existe un usuario con el username {row.Username}");

            child.RuleForEach(row => row!.Credencials)
              .WhereAsync(async field => await typeRepo.Data.AnyAsync(row => row.Id == field.CredentialType && row.AuthType == AuthenticationMethods.Single))
              .MustAsync(async (field, cancellationToken) =>
              {
                Result<Guid> app = await context.RetrieveAppContext(cancellationToken);
                return app.Success && !await singleRepo.Data.AnyAsync(row => row.Username == field.Username && row.AppId == app.Value, cancellationToken);
              })
              .WithErrorCode("Duplicated")
              .WithMessage((_, row) => $"Ya existe un usuario con el username {row.Username}");
          });
      }
    );

    When(
      row => row.UserInfo == null,
      () =>
      {
        RuleFor(row => row.UserRefId)
          .Must(field => field != null && field != Guid.Empty)
          .WithErrorCode("Invalid")
          .WithMessage("Debe haber alguna referencia de usuario - $.userRefId")
          .MustAsync(async (field, cancellationToken) => await userRepo.Data.AnyAsync(row => row.Id == field, cancellationToken))
          .WithErrorCode("NotFound")
          .WithMessage("Referencia del usuario no encontrada")
          .MustAsync(async (field, cancellationToken) =>
          {
            Result<Guid> app = await context.RetrieveAppContext(cancellationToken);
            return app.Success && !await userRepo.Data.AnyAsync(row => row.Id == field && row.Roles.Any(r => r.Role.AppId == app.Value), cancellationToken);
          })
          .WithMessage("El usuario ya esta añadido a la aplicación");
      }
    );
  }
}
