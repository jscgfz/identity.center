using System.Security.Claims;
using FluentValidation;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Extensions;

internal static class IdentityExtensions
{
  public static void ValidateAppContext<TDto>(this AbstractValidator<TDto> validator, IHttpContextAccessor http)
  {
    validator
      .RuleFor(row => row)
      .MustAsync(async (field, cancellationToken) => (await http.RetrieveAppContext(cancellationToken)).Success)
      .WithName("AppContext")
      .WithErrorCode("NotFound")
      .WithMessage("No se encontro el identificador de la aplicación");
  }

  public static async Task<Result<Guid>> RetrieveAppContext(this IHttpContextAccessor http, CancellationToken cancellationToken = default)
    => http.HttpContext?.User.FindFirstValue(IdentityClaimTypes.App) is string app &&
      Guid.TryParse(app, out Guid appId) &&
      await http.HttpContext.RequestServices.GetRequiredService<IIdentityRepository<App>>().Data.AnyAsync(row => row.Id == appId, cancellationToken)
      ? appId.Success()
      : Result.Result.Failure<Guid>(
        System.Net.HttpStatusCode.NotFound,
        new BaseError("AppContext.NotFound", "No se encontro el identificador de la aplicación")
      );
}
