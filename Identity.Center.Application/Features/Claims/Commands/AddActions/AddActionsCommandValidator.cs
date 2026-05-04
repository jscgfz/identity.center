using FluentValidation;
using Identity.Center.Domain.Common;

namespace Identity.Center.Application.Features.Claims.Commands.AddActions;

internal sealed class AddActionsCommandValidator : AbstractValidator<AddActionsCommand>
{
  public AddActionsCommandValidator()
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
