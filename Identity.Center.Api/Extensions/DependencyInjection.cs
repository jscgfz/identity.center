using System.Diagnostics;
using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Identity.Center.Api.Configuration;
using Identity.Center.Infrastructure.Configuration.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Identity.Center.Api.Extensions;

public static class DependencyInjection
{
  private static Action<OpenApiOptions> _options => options =>
  {
    options.AddDocumentTransformer((doc, context, cancellationToken) =>
    {
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
          JwtBearerDefaults.AuthenticationScheme,
          new()
          {
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Name = JwtBearerDefaults.AuthenticationScheme,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Description = "Ingreso con Json Web Token"
          }
        },
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
        { new() { Reference = new() { Id = JwtBearerDefaults.AuthenticationScheme, Type = ReferenceType.SecurityScheme } }, [] }
      });

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
        options.GroupNameFormat = "'v'VVV"; // Esto genera "v1", "v2", etc.
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
    app.MapOpenApi("/openapi/{documentName}/openapidoc.json");
    return app;
  }

  public static IEndpointRouteBuilder RegistryRoutes(this IEndpointRouteBuilder builder)
    => builder
      .RegistryRoutesOfType<IIdentityModule>();

  public static WebApplication WithSwagger(this WebApplication app)
  {
    app.UseSwaggerUI(options =>
    {
      IEnumerable<ApiVersionDescription> versions = app.DescribeApiVersions();
      foreach (ApiVersionDescription version in versions)
        options.SwaggerEndpoint($"/openapi/{version.GroupName}/openapidoc.json", version.GroupName);
      options.RoutePrefix = "reference";
    });
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
      .AddProblemDetails(options =>
      {
        options.CustomizeProblemDetails = context =>
        {
          context.ProblemDetails.Instance = context.HttpContext.Request.Path;
          context.ProblemDetails.Extensions.TryAdd("method", context.HttpContext.Request.Method);
          context.ProblemDetails.Extensions.TryAdd("host", $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}");
          context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
          Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
          context.ProblemDetails.Extensions.TryAdd("requestId", activity?.Id);
          if (context.Exception is Exception ex)
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
}
