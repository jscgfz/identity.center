using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Domain.Entities.Core.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Application.Features.Users.Commands.AddUser;

internal sealed class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
  public AddUserCommandValidator(IIdentityRepository<User> userRepo, IIdentityRepository<ContactInfo> contactRepo)
  {
    RuleFor(row => KeyValuePair.Create(row.DocumentType, row.DocumentNumber))
      .MustAsync(async (field, cancellationToken) => !await userRepo.Data.AnyAsync(row => row.DocumentType == field.Key && row.DocumentNumber == field.Value, cancellationToken))
      .WithName("DocumentData")
      .WithErrorCode("Duplicated")
      .WithMessage((ob, prop) => $"El documento {prop.Key} {prop.Value} ya esta registrada");

    RuleForEach(row => row.ContactInfo)
      .MustAsync(async (field, cancellationToken) => !await contactRepo.Data.AnyAsync(row => row.ContactType.ContactTypeKey == field.ContactTypeId && row.Value == field.Value, cancellationToken))
      .WithErrorCode("Duplicated")
      .WithMessage("La información del contacto esta duplicada");
  }
}
