using Identity.Center.Application.Abstractions.Managers;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Common.Options;
using Identity.Center.Domain.Enums;
using Identity.Center.Infrastructure.Configuration.Authentication;
using Identity.Center.Infrastructure.Configuration.Authorization;
using Identity.Center.Infrastructure.Hosting.Broker;
using Identity.Center.Infrastructure.Managers;
using Identity.Center.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
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
        ServiceDescriptor.Scoped(typeof(IIdentityRepository<>), typeof(IdentityRepository<>)),
        ServiceDescriptor.Scoped<IIdentityUnitOfWork, IdentityUnitOfWork>()
      ]);

    builder
      .Services
      .AddAuthorization();

    return builder;
  }

  public static WebApplicationBuilder WithBroker(this WebApplicationBuilder builder)
  {
    builder
      .Services
      .AddOptions<BrokerOptions>()
      .Bind(builder.Configuration.GetRequiredSection(nameof(BrokerOptions)))
      .ValidateOnStart();

    builder
      .Services
      .AddSingleton<IConnectionFactory>(sp =>
      {
        IOptionsMonitor<BrokerOptions> options = sp.GetRequiredService<IOptionsMonitor<BrokerOptions>>();
        ConnectionFactory factory = new()
        {
          HostName = options.CurrentValue.Host,
          UserName = options.CurrentValue.Username,
          Password = options.CurrentValue.Password,
          VirtualHost = options.CurrentValue.VirtualHost,
        };

        options.OnChange(opt =>
        {
          factory.HostName = opt.Host;
          factory.UserName = opt.Username;
          factory.Password = opt.Password;
          factory.VirtualHost = opt.VirtualHost;
        });

        return factory;
      });

    builder
      .Services
      .AddSingleton(sp =>
      {
        Task<IConnection> conn = sp.GetRequiredService<IConnectionFactory>().CreateConnectionAsync();
        return conn.GetAwaiter().GetResult();
      });

    builder
      .Services
      .AddHostedService<BrokerInitializer>();

    foreach (ContactTypes contact in Enum.GetValues<ContactTypes>())
    {
      builder
        .Services
        .AddKeyedSingleton(contact, (sp, key) => new BrokerMailingSwitchHandler(sp, (ContactTypes)key!));
      builder
        .Services
        .AddSingleton<IHostedService>(sp => sp.GetRequiredKeyedService<BrokerMailingSwitchHandler>(contact));
    }

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
