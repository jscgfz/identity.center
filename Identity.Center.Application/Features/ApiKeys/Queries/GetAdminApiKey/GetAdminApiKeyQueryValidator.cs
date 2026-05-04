using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Domain.Entities.Core.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Application.Features.ApiKeys.Queries.GetAdminApiKey;

internal sealed class GetAdminApiKeyQueryValidator : AbstractValidator<GetAdminApiKeyQuery>
{
  public GetAdminApiKeyQueryValidator(IIdentityRepository<ApiKey> repo)
  {
    RuleFor(row => row.SubjectId)
      .Must(row => row != Guid.Empty)
      .WithErrorCode("Invalid")
      .WithMessage("Identificador del sujeto invalido")
      .MustAsync(async (field, cancellationToken) => await repo.Data.AnyAsync(row => row.Id == field, cancellationToken))
      .WithErrorCode("NotFound")
      .OverridePropertyName("ApiKey")
      .WithMessage("Api key no encontrada");
  }
}
