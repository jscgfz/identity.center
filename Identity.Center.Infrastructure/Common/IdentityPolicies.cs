using System.ComponentModel.DataAnnotations;
using Identity.Center.Domain.Common;

namespace Identity.Center.Infrastructure.Common;

public static class IdentityPolicies
{
  public const string MfaPending = "jwt-mfa-pending";
  public const string Jwt = "jwt";
  public const string ApiKey = "api-key";
  public const string Root = "cprt-root";

  public static string FromClaim([RegularExpression(@"^[a-z]*\:[a-z]*$")] string claim)
    => IdentityCommons.FromClaim(claim);
}
