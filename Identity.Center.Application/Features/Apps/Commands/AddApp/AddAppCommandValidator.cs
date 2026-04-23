using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Application.Features.Apps.Commands.AddApp;

internal sealed class AddAppCommandValidator : AbstractValidator<AddAppCommand>
{
  public AddAppCommandValidator(
    IIdentityRepository<App> appRepo
  )
  {
    RuleFor(row => row.DomainName)
      .MustAsync(async (field, cancellationToken) => field is null || !await appRepo.Data.AnyAsync(row => row.DomainName == field, cancellationToken))
      .WithErrorCode("Duplicated")
      .WithMessage("El nombre de dominio debe ser único");

    RuleFor(row => row.Prefix)
      .MustAsync(async (field, cancellationToken) => !await appRepo.Data.AnyAsync(row => row.Prefix == field, cancellationToken))
      .WithErrorCode("Duplicated")
      .WithMessage("El prefijo debe ser único");

    RuleFor(row => row.Name)
      .MustAsync(async (field, cancellationToken) => !await appRepo.Data.AnyAsync(row => row.Name == field, cancellationToken))
      .WithErrorCode("Duplicated")
      .WithMessage("El nombre debe ser único");
  }
}
