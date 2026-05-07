using FluentValidation;
using Identity.Center.Application.Extensions;
using Microsoft.AspNetCore.Http;

namespace Identity.Center.Application.Features.Users.Queries.GetUsers;

internal sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
  public GetUsersQueryValidator(IHttpContextAccessor httpContext)
  {
    this.ValidateAppContext(httpContext);
  }
}
