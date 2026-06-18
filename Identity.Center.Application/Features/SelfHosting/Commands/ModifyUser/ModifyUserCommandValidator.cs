using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Domain.Entities.Core.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Application.Features.SelfHosting.Commands.ModifyUser;

internal sealed class ModifyUserCommandValidator : AbstractValidator<ModifyUserCommand>
{
  public ModifyUserCommandValidator(
    IIdentityRepository<User> userRepo
  )
  {
    RuleFor(row => row.UserId)
      .Must(field => field != Guid.Empty)
      .WithErrorCode("Invalid")
      .WithMessage("Identificador del usuario invalido")
      .MustAsync(async (field, cancellationToken) => await userRepo.Data.AnyAsync(row => row.Id == field, cancellationToken))
      .WithErrorCode("NotFound")
      .WithMessage("Usuario no encontrado");

    When(row => !string.IsNullOrWhiteSpace(row.Dto.DocumentNumber) && string.IsNullOrWhiteSpace(row.Dto.DocumentType), () =>
    {
      RuleFor(row => row)
        .MustAsync(async (field, cancellationToken) =>
        {
          string documentType = await userRepo.Data
            .Where(row => row.Id == field.UserId)
            .Select(row => row.DocumentType)
            .FirstAsync(cancellationToken);

          return !await userRepo.Data.AnyAsync(row => row.Id != field.UserId && row.DocumentNumber == field.Dto.DocumentNumber && row.DocumentType == documentType, cancellationToken);
        })
        .WithName("Document")
        .WithErrorCode("Invalid")
        .WithMessage("Ya existe un usuario con los datos del documento enviados");
    });

    When(row => string.IsNullOrWhiteSpace(row.Dto.DocumentNumber) && !string.IsNullOrWhiteSpace(row.Dto.DocumentType), () =>
    {
      RuleFor(row => row)
        .MustAsync(async (field, cancellationToken) =>
        {
          string documentNumber = await userRepo.Data
            .Where(row => row.Id == field.UserId)
            .Select(row => row.DocumentNumber)
            .FirstAsync(cancellationToken);

          return !await userRepo.Data.AnyAsync(row => row.Id != field.UserId && row.DocumentNumber == documentNumber && row.DocumentType == field.Dto.DocumentType, cancellationToken);
        })
        .WithName("Document")
        .WithErrorCode("Invalid")
        .WithMessage("Ya existe un usuario con los datos del documento enviados");
    });

    When(row => !string.IsNullOrWhiteSpace(row.Dto.DocumentNumber) && !string.IsNullOrWhiteSpace(row.Dto.DocumentType), () =>
    {
      RuleFor(row => row)
        .MustAsync(async (field, cancellationToken) => !await userRepo.Data.AnyAsync(row => row.Id != field.UserId && row.DocumentNumber == field.Dto.DocumentNumber && row.DocumentType == field.Dto.DocumentType, cancellationToken))
        .WithName("Document")
        .WithErrorCode("Invalid")
        .WithMessage("Ya existe un usuario con los datos del documento enviados");
    });
  }
}
