using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Application.Features.Claims.Commands.AddApiKeyClaims;

internal sealed class AddApiKeyClaimsCommandValidator : AbstractValidator<AddApiKeyClaimsCommand>
{
  public AddApiKeyClaimsCommandValidator(
    IIdentityRepository<ApiKey> apikeyRepo,
    IIdentityRepository<Group> groupRepo,
    IIdentityRepository<Domain.Entities.Core.Security.Action> actionRepo
  )
  {
    RuleFor(row => row.SubjectId)
      .Must(row => row != Guid.Empty)
      .WithErrorCode("Invalid")
      .WithMessage("Identificador de sujeto invalido")
      .MustAsync(async (field, cancellationToken) => await apikeyRepo.Data.AnyAsync(row => row.Id == field, cancellationToken))
      .WithErrorCode("NotFound")
      .WithMessage("No se encontró ninguna api key con el identificador del sujeto");

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
