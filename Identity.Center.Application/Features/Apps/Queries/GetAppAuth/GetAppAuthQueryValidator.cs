using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Application.Features.Apps.Queries.GetAppAuth;

internal sealed class GetAppAuthQueryValidator : AbstractValidator<GetAppAuthQuery>
{
  public GetAppAuthQueryValidator(IIdentityRepository<App> apprepo, IIdentityRepository<AppAuth> authrepo)
  {
    RuleFor(row => row.Id)
      .Must(row => row != Guid.Empty)
      .WithErrorCode("Invalid")
      .WithMessage("Identificador de la aplicación invalido")
      .MustAsync((field, cancellationToken) => apprepo.Data.AnyAsync(row => field == row.Id, cancellationToken))
      .WithErrorCode("NotFound")
      .OverridePropertyName("App")
      .WithMessage("Aplicación no encontrada")
      .MustAsync((field, cancellationToken) => authrepo.Data.AnyAsync(row => field == row.AppId, cancellationToken))
      .WithErrorCode("NotFound")
      .OverridePropertyName("AppAuth")
      .WithMessage("Información de seguridad no encontrada");
  }
}
