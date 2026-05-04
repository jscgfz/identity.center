using FluentValidation;
using Identity.Center.Domain.Common;

namespace Identity.Center.Application.Features.Claims.Commands.AddGroups;

internal sealed class AddGroupsCommandValidator : AbstractValidator<AddGroupsCommand>
{
  public AddGroupsCommandValidator()
  {
    RuleForEach(row => row.Cmd)
      .Must(row => !string.IsNullOrWhiteSpace(row.Name))
      .WithErrorCode("Empty")
      .OverridePropertyName("Name")
      .WithMessage("Nombre obligatorio")
      .Must(row => IdentityCommons.LowerRegex.IsMatch(row.Name))
      .WithErrorCode("LowerCase")
      .OverridePropertyName("Name")
      .WithMessage("El nombre debe estar en minúsculas")
      .Must(row => !string.IsNullOrWhiteSpace(row.Description))
      .WithErrorCode("Empty")
      .OverridePropertyName("Description")
      .WithMessage("Descripción obligatoria");
  }
}
