using Identity.Center.Domain.Common;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Enums;
using Identity.Center.Infrastructure.Common;
using Identity.Center.Infrastructure.Configuration.Authentication;
using Identity.Center.Persistence.Data.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Identity.Center.Infrastructure.Configuration.Authorization;

internal sealed class AuthorizationConfigurer : IConfigureOptions<AuthorizationOptions>
{
  public void Configure(AuthorizationOptions options)
  {
    options.AddPolicy(
          IdentityPolicies.MfaPending,
          new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireClaim(IdentityClaimTypes.Mfa, IdentityCommons.Serialize(MfaStates.Pending))
            .Build()
        );

    options.AddPolicy(
      IdentityPolicies.Jwt,
      new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .RequireClaim(IdentityClaimTypes.Mfa, IdentityCommons.Serialize(MfaStates.NotRequired), IdentityCommons.Serialize(MfaStates.Passed))
        .Build()
    );

    options.AddPolicy(
      IdentityPolicies.ApiKey,
      new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(ApiKeySchemeOptions.DefaultScheme)
        .RequireAuthenticatedUser()
        .Build()
    );

    options.AddPolicy(
      IdentityPolicies.Root,
      new AuthorizationPolicyBuilder()
        .RequireClaim(IdentityClaimTypes.Caim, "root")
        .Build()
    );
  }
}
