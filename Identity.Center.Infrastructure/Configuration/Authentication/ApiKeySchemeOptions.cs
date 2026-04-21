using Microsoft.AspNetCore.Authentication;

namespace Identity.Center.Infrastructure.Configuration.Authentication;

public sealed class ApiKeySchemeOptions : AuthenticationSchemeOptions
{
  public const string DefaultScheme = "IdentityApiKeyScheme";
  public string HeaderName { get; set; } = "x-api-key";
}