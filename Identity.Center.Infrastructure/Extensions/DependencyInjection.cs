using Identity.Center.Application.Abstractions.Managers;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Infrastructure.Configuration.Authentication;
using Identity.Center.Infrastructure.Configuration.Authorization;
using Identity.Center.Infrastructure.Managers;
using Identity.Center.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace Identity.Center.Infrastructure.Extensions;

public static class DependencyInjection
{
  public static WebApplicationBuilder WithAuth(this WebApplicationBuilder builder)
  {
    builder
      .Services
      .ConfigureOptions<JwtOptionsConfigurer>();

    builder
      .Services
      .AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer()
      .AddScheme<ApiKeySchemeOptions, ApiKeySchemeHandler>(
        ApiKeySchemeOptions.DefaultScheme,
        default
      );

    builder
      .Services
      .TryAddEnumerable([
        ServiceDescriptor.Transient<IClaimsTransformation, DbClaimsInjectionTrsformation>(),
        ServiceDescriptor.Transient<IClaimsManager, ClaimsManager>(),
        ServiceDescriptor.Scoped(typeof(IIdentityRepository<>), typeof(IdentityRepository<>))
      ]);

    builder
      .Services
      .AddAuthorization();

    return builder;
  }

  public static WebApplicationBuilder WithCache(this WebApplicationBuilder builder)
  {

    builder
      .Services
      .AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(sp.GetRequiredService<IConfiguration>().GetConnectionString(nameof(IRedis))!));
    builder
      .Services
      .AddSingleton(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

    return builder;
  }
}
