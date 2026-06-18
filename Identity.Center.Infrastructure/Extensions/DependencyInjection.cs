using Identity.Center.Application.Abstractions.Clients;
using Identity.Center.Application.Abstractions.Managers;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Common.Options;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Enums;
using Identity.Center.Infrastructure.Configuration.Authentication;
using Identity.Center.Infrastructure.Configuration.Authorization;
using Identity.Center.Infrastructure.Configuration.Configuration;
using Identity.Center.Infrastructure.Configuration.Logger;
using Identity.Center.Infrastructure.Hosting.Broker;
using Identity.Center.Infrastructure.Managers;
using Identity.Center.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Refit;
using StackExchange.Redis;

namespace Identity.Center.Infrastructure.Extensions;

public static class DependencyInjection
{
  public static WebApplicationBuilder WithConfigurationContext(this WebApplicationBuilder builder)
  {
    IConfigurationBuilder config = builder.Configuration;
    config.Add(
      new IdentityConfigurationSource(
        builder.Services.BuildServiceProvider(),
        new IdentityConfigurationChangeDetector(TimeSpan.FromMinutes(5))
      )
    )
    //.Add(
    //  new JsonConfigurationSource()
    //  {
    //    Optional = false,
    //    ReloadDelay = TimeSpan.FromMinutes(5).Milliseconds,
    //    ReloadOnChange = true,
    //    Path = Path.Combine(builder.Environment.ContentRootPath, "Templates", "notifications.json")
    //  }
    //)
    ;
    return builder;
  }

  public static WebApplicationBuilder WithAuth(this WebApplicationBuilder builder)
  {
    builder
      .Services
      .ConfigureOptions<JwtOptionsConfigurer>();

    builder
      .Services
      .AddAuthentication()
      .AddJwtBearer()
      .AddScheme<ApiKeySchemeOptions, ApiKeySchemeHandler>(
        ApiKeySchemeOptions.DefaultScheme,
        default
      );

    builder
      .Services
      .TryAddEnumerable([
        ServiceDescriptor.Transient<IClaimsTransformation, DbClaimsInjectionTransformation>(),
        ServiceDescriptor.Singleton<IAuthorizationMiddlewareResultHandler, IdentityAuthorizationHandler>(),
        ServiceDescriptor.Singleton<IAuthorizationPolicyProvider, BdClaimsPolicyProvider>(),
        ServiceDescriptor.Transient<IClaimsManager, ClaimsManager>(),
        ServiceDescriptor.Scoped(typeof(IIdentityRepository<>), typeof(IdentityRepository<>)),
        ServiceDescriptor.Scoped<IIdentityUnitOfWork, IdentityUnitOfWork>(),
        ServiceDescriptor.Scoped<ITokenManager, TokenManager>()
      ]);

    builder
      .Services
      .ConfigureOptions<AuthorizationConfigurer>();

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

  public static WebApplicationBuilder WithNotificationStrategy(this WebApplicationBuilder builder)
  {
    builder
      .Services
      .AddOptions<SmtpOptions>()
      .Bind(
        builder.Configuration
          .GetRequiredSection(nameof(SmtpOptions))
      )
      .ValidateOnStart();

    builder
      .Services
      .AddOptions<AlfrescoOptions>()
      .Bind(
        builder.Configuration
          .GetRequiredSection(nameof(AlfrescoOptions))
      )
      .ValidateOnStart();

    builder
      .Services
      .AddOptions<MasivianOptions>()
      .Bind(
        builder.Configuration
          .GetRequiredSection(nameof(MasivianOptions))
      )
      .ValidateOnStart();

    builder
      .Services
      .AddOptions<QdControlOptions>()
      .Bind(
        builder.Configuration
          .GetRequiredSection(nameof(QdControlOptions))
      )
      .ValidateOnStart();

    builder
      .Services
      .AddTransient<HttpLoginHandler>();

    builder
      .Services
      .AddRefitClient<IAlfrescoClient>()
      .ConfigureHttpClient((provider, client) =>
      {
        AlfrescoOptions options = provider.GetRequiredService<IOptionsMonitor<AlfrescoOptions>>().CurrentValue;
        client.BaseAddress = new Uri(options.BaseUrl);
        byte[] tokenBytes = IdentityCommons.Encoding.GetBytes($"{options.Username}:{options.Password}");
        client.DefaultRequestHeaders.Authorization = new("Basic", Convert.ToBase64String(tokenBytes));
      })
      .AddHttpMessageHandler<HttpLoginHandler>();

    builder
      .Services
      .AddRefitClient<IMasivianMailClient>()
      .ConfigureHttpClient((provider, client) =>
      {
        MasivianOptions options = provider.GetRequiredService<IOptionsMonitor<MasivianOptions>>().CurrentValue;
        client.BaseAddress = new Uri(options.EmailBaseUrl);
        byte[] tokenBytes = IdentityCommons.Encoding.GetBytes($"{options.Username}:{options.Password}");
        client.DefaultRequestHeaders.Authorization = new("Basic", Convert.ToBase64String(tokenBytes));
      })
      .AddHttpMessageHandler<HttpLoginHandler>();

    builder
      .Services
      .AddRefitClient<IMasivianSmsClient>()
      .ConfigureHttpClient((provider, client) =>
      {
        MasivianOptions options = provider.GetRequiredService<IOptionsMonitor<MasivianOptions>>().CurrentValue;
        client.BaseAddress = new Uri(options.SmsBaseUrl);
        byte[] tokenBytes = IdentityCommons.Encoding.GetBytes($"{options.Username}:{options.Password}");
        client.DefaultRequestHeaders.Authorization = new("Basic", Convert.ToBase64String(tokenBytes));
      })
      .AddHttpMessageHandler<HttpLoginHandler>();

    builder
      .Services
      .AddRefitClient<IQdControlClient>()
      .ConfigureHttpClient((provider, client) =>
      {
        QdControlOptions options = provider.GetRequiredService<IOptionsMonitor<QdControlOptions>>().CurrentValue;
        client.BaseAddress = new Uri(options.BaseUrl);
      })
      .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
      {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
      })
      .AddHttpMessageHandler<HttpLoginHandler>();

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
