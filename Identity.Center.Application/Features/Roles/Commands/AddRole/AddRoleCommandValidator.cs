using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Application.Features.Roles.Commands.AddRole;

internal sealed class AddRoleCommandValidator : AbstractValidator<AddRoleCommand>
{
  public AddRoleCommandValidator(
    IIdentityRepository<App> appRepo,
    IIdentityRepository<Role> roleRepo,
    IIdentityRepository<Group> groupRepo,
    IIdentityRepository<Domain.Entities.Core.Security.Action> actionRepo
  )
  {
    RuleFor(row => row.AppId)
      .MustAsync(async (field, cancellationToken) => await appRepo.Data.AnyAsync(row => row.Id == field, cancellationToken))
      .WithErrorCode("NotFound")
      .WithName("App")
      .WithMessage("Aplicación no encontrada");

    RuleFor(row => row)
      .MustAsync(async (ob, cancellationToken) => !await roleRepo.Data.AnyAsync(row => row.AppId == ob.AppId && row.Name == ob.Name, cancellationToken))
      .WithName("Role")
      .WithErrorCode("Duplicated")
      .WithMessage((_, ob) => $"El rol con nombre {ob.Name} ya esta registrado para la app con el identificador {ob.AppId}");

    When(
      row => !string.IsNullOrWhiteSpace(row.DomainName),
      () =>
      {
        RuleFor(row => row.DomainName)
          .MustAsync(async (field, cancellationToken) => !await roleRepo.Data.AnyAsync(row => row.DomainName == field, cancellationToken))
          .WithErrorCode("Duplicated")
          .WithMessage("Ya se encuentra registrado un rol con el nombre de dominio");
      }
    );

    When(
      row => row.ActiveDirectoryMandatory,
      () =>
      {
        RuleFor(row => row.DomainName)
          .Must(field => !string.IsNullOrWhiteSpace(field))
          .WithErrorCode("Empty")
          .WithMessage("Cuando la validación por AD es obligatoria el nombre de dominio es requerido");
      }
    );

    RuleFor(row => row.Claims)
      .Must(field => field != null && field.Any())
      .WithErrorCode("Empty")
      .WithMessage("Claims obligatorios");

    RuleForEach(row => row.Claims)
      .Must(IdentityCommons.IsValidClaim)
      .OverridePropertyName("Claims")
      .WithErrorCode("Invalid")
      .WithMessage("formato de claim invalido")
      .MustAsync(async (field, cancellationToken) =>
      {
        KeyValuePair<string, string> parts = IdentityCommons.Deserialize(field);
        return await groupRepo.Data.AnyAsync(row => row.Name == parts.Key, cancellationToken);
      })
      .WithErrorCode("NotFound")
      .OverridePropertyName("Claims.Group")
      .WithMessage("Grupo del claim no configurado")
      .MustAsync(async (field, cancellationToken) =>
      {
        KeyValuePair<string, string> parts = IdentityCommons.Deserialize(field);
        return await actionRepo.Data.AnyAsync(row => row.Name == parts.Value, cancellationToken);
      })
      .WithErrorCode("NotFound")
      .OverridePropertyName("Claims.Action")
      .WithMessage("Acción del claim no configurada");
  }
}
