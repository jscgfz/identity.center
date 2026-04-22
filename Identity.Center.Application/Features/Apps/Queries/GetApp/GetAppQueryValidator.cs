using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Application.Features.Apps.Queries.GetApp;
internal sealed class GetAppQueryValidator : AbstractValidator<GetAppQuery>
{
  public GetAppQueryValidator(IIdentityRepository<App> repo)
  {
    RuleFor(row => row.Id)
      .Must(row => row != Guid.Empty)
      .WithErrorCode("Invalid")
      .WithMessage("Identificador de la aplicación invalido")
      .MustAsync((row, cancellationToken) => repo.Data.AnyAsync(row => row.Id == row.Id, cancellationToken))
      .WithErrorCode("NotFound")
      .OverridePropertyName("App")
      .WithMessage("Aplicación no encontrada");
  }
}
