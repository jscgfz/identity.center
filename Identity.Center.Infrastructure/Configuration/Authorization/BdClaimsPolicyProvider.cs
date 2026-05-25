using Identity.Center.Domain.Common;
using Identity.Center.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Identity.Center.Infrastructure.Configuration.Authorization;

internal sealed class BdClaimsPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
  public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
  {
    AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);
    if (policy == null && IdentityCommons.ValidatePolicyFromClaim(policyName, out string? claim))
      policy = new AuthorizationPolicyBuilder()
        .RequireClaim(IdentityClaimTypes.Caim, claim)
        .Build();

    return policy;
  }
}
