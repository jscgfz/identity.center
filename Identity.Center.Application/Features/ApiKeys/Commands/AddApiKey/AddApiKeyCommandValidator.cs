using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;
using Action = Identity.Center.Domain.Entities.Core.Security.Action;

namespace Identity.Center.Application.Features.ApiKeys.Commands.AddApiKey;

internal sealed class AddApiKeyCommandValidator : AbstractValidator<AddApiKeyCommand>
{
  public AddApiKeyCommandValidator(
    IIdentityRepository<App> appRepo,
    IIdentityRepository<ApiKey> apikeyRepo,
    IIdentityRepository<Group> groupRepo,
    IIdentityRepository<Action> actionRepo
  )
  {
    RuleFor(row => row.AppId)
      .MustAsync(async (field, cancellationToken) => await appRepo.Data.AnyAsync(row => row.Id == field, cancellationToken))
      .WithErrorCode("NotFound")
      .WithMessage("No se encontró ninguna aplicación con el identificador");
    RuleFor(row => row.Name)
      .MustAsync(async (field, cancellationToken) => !await apikeyRepo.Data.AnyAsync(row => row.Name == field, cancellationToken))
      .WithErrorCode("Duplicated")
      .WithMessage("El nombre de la apiKey debe ser único");
    RuleFor(row => row.Claims)
      .NotEmpty();
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
