using System.Reflection;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Constants;
using Identity.Center.Infrastructure.Configuration.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Center.Api.Configuration.Authorization;

public sealed class IdentityPolicyBuilder
{
  private static readonly AuthorizationPolicyBuilder Policy = new();
  private readonly List<AuthorizationPolicy> _policies;
  private IdentityPolicyBuilder()
    => _policies = [];

  public static IdentityPolicyBuilder Merged(params IEnumerable<IdentityPolicyBuilder> builders)
  {
    IdentityPolicyBuilder mainBuilder = new();
    foreach (IdentityPolicyBuilder builder in builders)
      mainBuilder.Append(builder);

    return mainBuilder;
  }

  public static IdentityPolicyBuilder Empty => new();

  public IdentityPolicyBuilder Append(IdentityPolicyBuilder builder)
  {
    FieldInfo fieldinfo = GetType().GetField(nameof(_policies), BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new ArgumentNullException(nameof(builder));
    List<AuthorizationPolicy> policies = (List<AuthorizationPolicy>?)fieldinfo.GetValue(builder) ?? throw new ArgumentNullException(nameof(builder));
    _policies.AddRange(policies);
    return this;
  }

  public IdentityPolicyBuilder AllowJtw()
  {
    _policies.Add(
      Policy
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build()
    );
    return this;
  }

  public IdentityPolicyBuilder AllowApiKeys()
  {
    _policies.Add(
      Policy
        .AddAuthenticationSchemes(ApiKeySchemeOptions.DefaultScheme)
        .RequireAuthenticatedUser()
        .Build()
    );
    return this;
  }

  public IdentityPolicyBuilder RequireClaims(params IEnumerable<string> claims)
  {
    if (!claims.All(IdentityCommons.IsValidClaim))
      throw new ArgumentException("Invalid claims", nameof(claims));
    _policies.Add(
      Policy
        .RequireClaim(IdentityClaimTypes.Caim, claims)
        .Build()
    );
    return this;
  }

  public IdentityPolicyBuilder RequireRoot()
  {
    _policies.Add(
      Policy
        .RequireClaim(IdentityClaimTypes.Caim, "root")
        .Build()
    );
    return this;
  }

  public void Apply<TRoute>(TRoute route)
    where TRoute : IEndpointConventionBuilder
  {
    if (!_policies.Any())
      throw new ArgumentException("Politicas obligatorias", nameof(route));

    AuthorizationPolicyBuilder builder = new();
    foreach (AuthorizationPolicy policy in _policies)
      builder.Combine(policy);

    route
      .RequireAuthorization(
        builder.Build()
      );
  }
}
