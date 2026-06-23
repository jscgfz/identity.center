using System.Diagnostics;
using System.Reflection;
using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Destructurama;
using Identity.Center.Api.Configuration;
using Identity.Center.Api.Configuration.Endpoints;
using Identity.Center.Application.Common.Options;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Constants;
using Identity.Center.Infrastructure.Configuration.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Cors;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Identity.Center.Api.Extensions;

public static class DependencyInjection
{
  private static Action<OpenApiOptions> _options => options =>
  {
    options.AddDocumentTransformer((doc, context, cancellationToken) =>
    {
      if (
        context.ApplicationServices
          .GetRequiredService<IConfiguration>()
          .GetSection(nameof(doc.Servers)).Get<IEnumerable<OpenApiServer>>() is IList<OpenApiServer> servers
      )
        doc.Servers = servers;

      doc.Info = new()
      {
        Title = "Identity Center Services",
        Contact = new()
        {
          Email = "jhon.cubillos@finanzauto.com.co",
          Name = "Jhon Sebastián Cubillos Gonzalez"
        },
        Description = "Servicios de gestión de identidad y control de permisos de los diferentes sitemas de la compañías",
      };
      doc.Components ??= new();
      doc.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>()
      {
        {
          $"{ApiKeySchemeOptions.DefaultScheme} Subject".ToLower(),
          new()
          {
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Name = ApiKeySchemeOptions.SubjectHeaderName,
            Scheme = $"{ApiKeySchemeOptions.DefaultScheme} Subject".ToLower(),
            Description = "Identificador del sujeto"
          }
        },
        {
          ApiKeySchemeOptions.DefaultScheme,
          new()
          {
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Name = ApiKeySchemeOptions.HeaderName,
            Scheme = ApiKeySchemeOptions.DefaultScheme,
            Description = "Api key del sujeto"
          }
        }
      };

      doc.SecurityRequirements.Add(new()
      {
        { new() { Reference = new() { Id = $"{ApiKeySchemeOptions.DefaultScheme} Subject".ToLower(), Type = ReferenceType.SecurityScheme } }, [] },
        { new() { Reference = new() { Id = ApiKeySchemeOptions.DefaultScheme, Type = ReferenceType.SecurityScheme } }, [] }
      });

      return Task.CompletedTask;
    });
  };

  public static WebApplicationBuilder WithOpenApiDocumentation(this WebApplicationBuilder builder, string? documentName = null)
  {
    builder
      .Services
      .AddEndpointsApiExplorer();

    EnvironmentOptions? envOptions = builder.Configuration
          .GetSection(nameof(EnvironmentOptions))
          .Get<EnvironmentOptions>();

    if (envOptions != null && envOptions.IsDevEnvironment(builder.Environment))
      _ = documentName switch
      {
        null => builder.Services.AddOpenApi(_options),
        _ => builder.Services.AddOpenApi(documentName, _options)
      };

    return builder;
  }

  public static WebApplicationBuilder WithVersioning(this WebApplicationBuilder builder)
  {
    builder
      .Services
      .AddApiVersioning(options =>
      {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
      })
      .AddApiExplorer(options =>
      {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
      });
    return builder;
  }

  public static WebApplication WithAuth(this WebApplication app)
  {
    app.UseAuthentication();
    app.UseAuthorization();
    return app;
  }

  public static WebApplication WithOpenApiDocumentation(this WebApplication app)
  {
    EnvironmentOptions? envOptions = app.Configuration
          .GetSection(nameof(EnvironmentOptions))
          .Get<EnvironmentOptions>();
    if (envOptions != null && envOptions.IsDevEnvironment(app.Environment))
      app.MapOpenApi("/openapi/{documentName}/openapidoc.json");

    return app;
  }

  public static IEndpointRouteBuilder RegistryRoutes(this IEndpointRouteBuilder builder)
    => builder
      .RegistryRoutesOfType<IIdentityModule>();

  public static WebApplication WithSwagger(this WebApplication app)
  {
    EnvironmentOptions? envOptions = app.Configuration
          .GetSection(nameof(EnvironmentOptions))
          .Get<EnvironmentOptions>();

    if (envOptions != null && envOptions.IsDevEnvironment(app.Environment))
      app.UseSwaggerUI(options =>
      {
        IEnumerable<ApiVersionDescription> versions = app.DescribeApiVersions();
        foreach (ApiVersionDescription version in versions)
          options.SwaggerEndpoint($"/openapi/{version.GroupName}/openapidoc.json", version.GroupName);
        options.RoutePrefix = "reference";
        options.DocumentTitle = "identity-center-services";
        options.DisplayRequestDuration();
        options.DisplayOperationId();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.HeadContent = "<link rel='icon' type='image/ico' href='https://www.finanzauto.com.co/portal/icon.ico' sizes='32x32' />";
        options.HeadContent += "<style>html.dark-mode .swagger-ui .opblock-description-wrapper, .swagger-ui .opblock-external-docs-wrapper, .swagger-ui .opblock-title_normal {\r\n    color: #bdc6dd;\r\n}\r\nhtml.dark-mode .swagger-ui .opblock.opblock-put .opblock-section-header {background: #9a5b3e47;border-bottom: 1px solid #523524;border-top: 1px solid #523524;}</style>";
        options.ConfigObject.AdditionalItems["cdn_url"] = "https://cdn.jsdelivr.net/npm/swagger-ui-dist@5/";
      });

    app
      .UseAntiforgery();

    return app;
  }

  public static IEndpointRouteBuilder RegistryRoutesOfType<TType>(this IEndpointRouteBuilder builder)
    where TType : IRouterModule
  {
    Assembly assembly = Assembly.GetExecutingAssembly();
    foreach (Type routerType in assembly.GetTypes().Where(t => t is { IsInterface: false, IsAbstract: false } && t.GetInterfaces().Contains(typeof(TType))))
      ((TType)Activator.CreateInstance(routerType)!).Registry(builder);

    return builder;
  }

  public static WebApplicationBuilder WithProblemDetails(this WebApplicationBuilder builder)
  {
    builder
      .Services
      .AddHttpContextAccessor();

    builder
      .Services
      .AddAntiforgery();

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
      options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
      options.KnownProxies.Clear();
      options.KnownNetworks.Clear();
    });

    builder
      .Services
      .AddProblemDetails(options =>
      {
        EnvironmentOptions? envOptions = builder.Configuration
          .GetSection(nameof(EnvironmentOptions))
          .Get<EnvironmentOptions>();

        options.CustomizeProblemDetails = context =>
        {
          context.ProblemDetails.Instance = context.HttpContext.Request.Path;
          context.ProblemDetails.Extensions.TryAdd("method", context.HttpContext.Request.Method);
          context.ProblemDetails.Extensions.TryAdd("host", $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}");
          context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
          Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
          context.ProblemDetails.Extensions.TryAdd("requestId", activity?.Id);
          if (context.Exception is Exception ex && envOptions != null && envOptions.IsDevEnvironment(builder.Environment))
          {
            context.ProblemDetails.Detail = ex.Message;
            context.ProblemDetails.Type = ex.HelpLink;
            context.ProblemDetails.Extensions.TryAdd("source", ex.Source);
            context.ProblemDetails.Extensions.TryAdd("stackTrace", ex.StackTrace);
          }
          context.ProblemDetails.Extensions = context.ProblemDetails.Extensions
            .Reverse()
            .ToDictionary();
        };
      });

    return builder;
  }

  public static TBuilder AddRequirement<TBuilder>(this TBuilder builder, string type, string value)
    where TBuilder : IEndpointConventionBuilder
  {
    builder.Add(b =>
    {
      b.Metadata.Add(new RequirementLabel(type, value));
    });
    return builder;
  }

  public static TBuilder BuildRequirements<TBuilder>(this TBuilder builder, string? description = null)
    where TBuilder : IEndpointConventionBuilder
  {
    builder.Add(b =>
    {
      StringBuilder stringBuilder = new();
      IEnumerable<RequirementLabel> requirements = b.Metadata.OfType<RequirementLabel>();
      if (requirements.Any())
      {
        stringBuilder.AppendLine("### Requerimientos");
        stringBuilder.AppendLine(string.Empty);
        stringBuilder.AppendLine("| Tipo | Valor |");
        stringBuilder.AppendLine("| :--- | :--- |");
        foreach (RequirementLabel requirement in requirements.OrderBy(r => r.Type + "-" + r.Value))
          stringBuilder.AppendLine($"| {requirement.Type} | {requirement.Value} |");

      }
      if (!string.IsNullOrEmpty(description))
      {
        stringBuilder.AppendLine("### Descripción");
        stringBuilder.AppendLine(string.Empty);
        stringBuilder.AppendLine(description);
      }

      string data = stringBuilder.ToString();
      if (!string.IsNullOrWhiteSpace(data))
        b.Metadata.Add(new EndpointDescriptionAttribute(data));

    });
    return builder;
  }

  public static WebApplication WithNotificationTemplatesBrowser(this WebApplication app)
  {
    if (app.Environment.IsDevelopment())
    {
      app.UseDirectoryBrowser(
        new DirectoryBrowserOptions()
        {
          RequestPath = "/notifications/templates",
          FileProvider = new PhysicalFileProvider(
            Path.Combine(app.Environment.ContentRootPath, "Templates", "Notifications")
          )
        }
      );

      app.UseStaticFiles(
        new StaticFileOptions()
        {
          RequestPath = "/notifications/templates",
          FileProvider = new PhysicalFileProvider(
            Path.Combine(app.Environment.ContentRootPath, "Templates", "Notifications")
          )
        }
      );
    }
    return app;
  }

  public static RouteHandlerBuilder AddAuthProduces(this RouteHandlerBuilder builder)
    => builder
      .ProducesProblem(StatusCodes.Status401Unauthorized)
      .ProducesProblem(StatusCodes.Status403Forbidden)
      .ProducesProblem(StatusCodes.Status400BadRequest);

  public static TBuilder BuildRequirementsDoc<TBuilder>(this TBuilder builder, string? description = null)
    where TBuilder : IEndpointConventionBuilder
  {
    builder.Add(options =>
    {
      StringBuilder authMatrix = new();
      if (!string.IsNullOrWhiteSpace(description))
      {
        options.Metadata.Add(new EndpointSummaryAttribute(description));
        authMatrix.AppendLine("**Descripción**");
        authMatrix.AppendLine($"  - {description}");
        authMatrix.AppendLine();
      }
      if (options.Metadata.OfType<AuthorizeAttribute>().Where(a => !string.IsNullOrWhiteSpace(a.Policy)).Any())
      {
        authMatrix.AppendLine("**Politicas**");
        foreach (AuthorizeAttribute attr in options.Metadata.OfType<AuthorizeAttribute>().Where(a => !string.IsNullOrWhiteSpace(a.Policy)))
        {
          string name = IdentityCommons.ValidatePolicyFromClaim(attr.Policy!, out string? claim) ? $"{IdentityClaimTypes.Caim} - {claim}" : attr.Policy!.ToLower();
          authMatrix.AppendLine($" - {name}");
        }
      }
      if (authMatrix.Length > 0)
        options.Metadata.Add(new EndpointDescriptionAttribute(authMatrix.ToString()));
    });
    return builder;
  }

  public static WebApplicationBuilder WithCors(this WebApplicationBuilder builder)
  {
    builder
      .Services
      .AddCors(options =>
      {
        options
          .AddDefaultPolicy(policy =>
          {
            policy
              .SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
          });
      });

    return builder;
  }

  public static WebApplicationBuilder WithLoggin(this WebApplicationBuilder builder)
  {
    builder
      .Services
      .AddSerilog((provider, options) =>
      {
        options
          .Destructure
          .SystemTextJsonTypes()
          .ReadFrom
          .Configuration(provider.GetRequiredService<IConfiguration>());
      });



    builder
      .Services
      .AddOpenTelemetry()
      .ConfigureResource(resource =>
      {
        resource
          .AddService("identity", new Version(1, 0, 1, 0).ToString());
      })
      .WithTracing(tracing =>
      {
        tracing
          .AddSource("identity")
          .AddAspNetCoreInstrumentation(options =>
          {
            options.RecordException = true;
          })
          .AddHttpClientInstrumentation(options =>
          {
            options.RecordException = true;
          })
          .AddEntityFrameworkCoreInstrumentation();
      });

    return builder;
  }
}
