using Microsoft.AspNetCore.Authentication;

namespace Identity.Center.Infrastructure.Configuration.Authentication;

public sealed class ApiKeySchemeOptions : AuthenticationSchemeOptions
{
  public const string DefaultScheme = "IdentityApiKey";
  public const string HeaderName = "x-api-key";
  public const string SubjectHeaderName = "x-api-subject";
}