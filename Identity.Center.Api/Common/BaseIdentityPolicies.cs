using Identity.Center.Api.Configuration.Authorization;

namespace Identity.Center.Api.Common;

public static class BaseIdentityPolicies
{
  public static IdentityPolicyBuilder Jwt => IdentityPolicyBuilder.Empty.AllowJtw();
  public static IdentityPolicyBuilder ApiKey => IdentityPolicyBuilder.Empty.AllowApiKeys();
}
