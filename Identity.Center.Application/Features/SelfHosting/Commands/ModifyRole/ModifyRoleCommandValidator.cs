using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Application.Features.SelfHosting.Commands.ModifyRole;

internal sealed class ModifyRoleCommandValidator : AbstractValidator<ModifyRoleCommand>
{
  public ModifyRoleCommandValidator(
    IIdentityRepository<ChangeControl> controlRepo
  )
  {
    RuleFor(row => row.Status)
      .IsInEnum()
      .WithMessage("Cambio de estado obligatorio")
      .Must(field => new[] { ChangeControlStates.Rejected, ChangeControlStates.Approved }.Contains(field))
      .WithErrorCode("Invalid")
      .WithMessage("Cambio de estado invalido");

    RuleFor(row => row.ChangeControlId)
      .NotEmpty()
      .WithMessage("Identificador de la solicitud obligatoria")
      .MustAsync(async (field, cancellationToken) => await controlRepo.Data.AnyAsync(row => row.Id == field && row.Status == ChangeControlStates.Pending, cancellationToken))
      .WithName("ChangeControl")
      .WithErrorCode("Finalized")
      .WithMessage("La solicitud ya ha sido gestionada");
  }
}
