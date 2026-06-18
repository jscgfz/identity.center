using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Extensions;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Application.Features.SelfHosting.Commands.AddRequestRole;

internal sealed class AddRequestRoleCommandValidator : AbstractValidator<AddRequestRoleCommand>
{
  public AddRequestRoleCommandValidator(
    IHttpContextAccessor context,
    IIdentityRepository<Role> roleRepo,
    IIdentityRepository<Group> groupRepo,
    IIdentityRepository<ChangeControl> changeRepo,
    IIdentityRepository<Domain.Entities.Core.Security.Action> actionRepo
  )
  {
    RuleFor(row => row.RoleId)
      .MustAsync(async (field, cancellationToken) =>
      {
        Result<Guid> app = await context.RetrieveAppContext(cancellationToken);
        return app.Success && await roleRepo.Data.AnyAsync(row => row.Id == field && row.AppId == app.Value, cancellationToken);
      })
      .WithName("Role")
      .WithErrorCode("NotFoud")
      .WithMessage("Rol no encontrado")
      .MustAsync(async (field, cancellationToken) => !await changeRepo.Data.AnyAsync(row => row.RoleId == field && row.Status == Domain.Enums.ChangeControlStates.Pending, cancellationToken))
      .WithName("Request")
      .WithErrorCode("Pending")
      .WithMessage("Ya hay una solicitud de esición pendiente para este rol");

    When(row => !string.IsNullOrWhiteSpace(row.Dto.Name), () =>
    {
      RuleFor(row => row)
        .MustAsync(async (field, cancellationToken) =>
        {
          Result<Guid> app = await context.RetrieveAppContext(cancellationToken);
          return app.Success && !await roleRepo.Data.AnyAsync(row => row.Id != field.RoleId && row.AppId == app.Value && row.Name == field.Dto.Name, cancellationToken);
        })
        .WithName("Name")
        .WithErrorCode("Duplicated")
        .WithMessage((_, p) => $"Ya existe un rol con el nombre {p.Dto.Name}");
    });

    When(row => !string.IsNullOrWhiteSpace(row.Dto.DomainName), () =>
    {
      RuleFor(row => row)
        .MustAsync(async (field, cancellationToken) =>
        {
          Result<Guid> app = await context.RetrieveAppContext(cancellationToken);
          return app.Success && !await roleRepo.Data.AnyAsync(row => row.Id != field.RoleId && row.AppId == app.Value && row.DomainName == field.Dto.DomainName, cancellationToken);
        })
        .WithName("Name")
        .WithErrorCode("Duplicated")
        .WithMessage((_, p) => $"Ya existe un rol con el nombre de dominio {p.Dto.DomainName}");
    });

    When(row => row.Dto.Claims != null, () =>
    {
      RuleFor(row => row.Dto.Claims)
        .NotEmpty()
        .WithMessage("Debe tener al menos un claim");

      RuleForEach(row => row.Dto.Claims)
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
    });

    RuleFor(row => row)
      .MustAsync(async (field, cancellationToken) =>
      {
        RolePictureDto? currentPicture = await roleRepo.Data
          .Where(row => row.Id == field.RoleId)
          .Select(row => new RolePictureDto(
            row.Name,
            row.Description,
            row.DomainName,
            row.ActiveDirectoryMandatory,
            row.Root,
            row.Claims.Select(c => c.Claim.Group.Name + ":" + c.Claim.Action.Name)
          ))
          .FirstOrDefaultAsync(cancellationToken);

        if (currentPicture == null) return false;

        RolePictureDto requestPicture = new(
          field.Dto.Name,
          field.Dto.Description,
          field.Dto.DomainName,
          field.Dto.ActiveDiretoryMandatory,
          field.Dto.Root,
          field.Dto.Claims
        );

        ValueComparison<string> name = new(currentPicture.Name!, requestPicture.Name);
        ValueComparison<string?> description = new(currentPicture.Description, requestPicture.Description);
        ValueComparison<string?> domainName = new(currentPicture.DomainName, requestPicture.DomainName);
        ValueComparison<bool?> activeDirectory = new(currentPicture.ActiveDiretoryMandatory, requestPicture.ActiveDiretoryMandatory);
        ValueComparison<bool?> root = new(currentPicture.Root, requestPicture.Root);
        ValueComparison<IEnumerable<string>> claims = new(currentPicture.Claims!, requestPicture.Claims);

        return new[]
        {
          name.HasChange,
          description.HasChange,
          domainName.HasChange,
          activeDirectory.HasChange,
          claims.HasChange,
          root.HasChange
        }.Any(r => r);
      })
      .WithName("Request")
      .WithErrorCode("NotValid")
      .WithMessage("Solicitud invalida, no se refleja ningun cambio");
  }
}
