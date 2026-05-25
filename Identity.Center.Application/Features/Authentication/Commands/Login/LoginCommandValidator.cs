using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Application.Features.Authentication.Commands.Login;

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
  public LoginCommandValidator(IIdentityRepository<App> appRepo)
  {
    RuleFor(row => row.AppId)
      .Must(row => row != Guid.Empty)
      .MustAsync(async (field, cancellationToken) => await appRepo.Data.AnyAsync(row => row.Id == field, cancellationToken))
      .WithName("App")
      .WithErrorCode("NotFound")
      .WithMessage("No se encontró información de la alpicación");
  }
}
