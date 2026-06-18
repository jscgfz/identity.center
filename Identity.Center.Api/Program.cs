using Asp.Versioning.Builder;
using Identity.Center.Api.Extensions;
using Identity.Center.Application.Extensions;
using Identity.Center.Infrastructure.Configuration.Logger;
using Identity.Center.Infrastructure.Extensions;
using Identity.Center.Persistence.Extensions;

WebApplication app = WebApplication
  .CreateBuilder(args)
  .WithLoggin()
  .WithProblemDetails()
  .WithAuth()
  .WithIdentityPersistence()
  .WithCache()
  .WithVersioning()
  .WithOpenApiDocumentation("v1")
  .WithResultExtensions()
  .WithConfigurationContext()
  .WithNotificationStrategy()
  .WithBroker()
  .WithCors()
  .Build()
  //.WithNotificationTemplatesBrowser()
  .WithOpenApiDocumentation()
  .WithAuth();

ApiVersionSet set = app
  .NewApiVersionSet()
  .HasApiVersion(new(1))
  .ReportApiVersions()
  .Build();

app
  .UseCors();

app
  .WithSwagger()
  .MapGroup("/api/v{version:apiVersion}")
  .WithApiVersionSet(set)
  .RegistryRoutes();

app
  .UseMiddleware<RequestLogginHandler>();

app
  .Run();