using System.Security.Claims;
using System.Text.Encodings.Web;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Common.Models.Cryptography;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Persistence.Data.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Identity.Center.Infrastructure.Configuration.Authentication;

public sealed class ApiKeySchemeHandler(
  IOptionsMonitor<ApiKeySchemeOptions> option,
  ILoggerFactory logger,
  UrlEncoder encoder
) : AuthenticationHandler<ApiKeySchemeOptions>(option, logger, encoder)
{
  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    IdentityContext context = Request.HttpContext.RequestServices.GetRequiredService<IdentityContext>();
    CancellationToken cancellationToken = Request.HttpContext.RequestAborted;

    if (
      !Request.Headers.TryGetValue(ApiKeySchemeOptions.HeaderName, out StringValues header) ||
      header.FirstOrDefault() is not string apiKey
    )
      return AuthenticateResult.Fail(
        new NullReferenceException(
          $"{ApiKeySchemeOptions.HeaderName} Header not Found"
        )
      );

    if (
      !Request.Headers.TryGetValue(ApiKeySchemeOptions.SubjectHeaderName, out StringValues subjectHeader) ||
      subjectHeader.FirstOrDefault() is not string subject
    )
      return AuthenticateResult.Fail(
        new NullReferenceException(
          $"{ApiKeySchemeOptions.SubjectHeaderName} Header not Found"
        )
      );

    if (
      !Guid.TryParse(subject, out Guid subjectId) ||
      !await context.Set<ApiKey>().AnyAsync(row => row.Id == subjectId, cancellationToken)
    )
      return AuthenticateResult.Fail(
        new InvalidDataException(
          $"Invalid subject ${subject}"
        )
      );

    ApiKey dbApikey = await context.Set<ApiKey>()
      .AsNoTracking()
      .FirstAsync(row => row.Id == subjectId, cancellationToken);

    HashValidationResponse hashValidation = await IdentityCommons.ValidateHash(
      new(
        apiKey,
        dbApikey.Hash,
        dbApikey.Salt
      ),
      cancellationToken
    );

    if (!hashValidation.Success)
      return AuthenticateResult.Fail(
        new UnauthorizedAccessException(
          "Invalid ApiKey"
        )
      );

    IEnumerable<Claim> initialClaims = [
      new Claim(ClaimTypes.NameIdentifier, subjectId.ToString("N")),
      new Claim(IdentityClaimTypes.App, dbApikey.AppId.ToString("N")),
    ];

    ClaimsIdentity identity = new(initialClaims, nameof(ApiKeySchemeHandler));
    ClaimsPrincipal principal = new(identity);
    AuthenticationTicket ticket = new(principal, Scheme.Name);

    return AuthenticateResult.Success(ticket);
  }
}
