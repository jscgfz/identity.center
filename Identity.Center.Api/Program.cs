using Asp.Versioning.Builder;
using Identity.Center.Api.Extensions;
using Identity.Center.Application.Extensions;
using Identity.Center.Infrastructure.Extensions;
using Identity.Center.Persistence.Extensions;

WebApplication app = WebApplication
  .CreateBuilder(args)
  .WithIdentityPersistence()
  .WithAuth()
  .WithCache()
  .WithVersioning()
  .WithOpenApiDocumentation("v1")
  .WithProblemDetails()
  .WithResultExtensions()
  .WithBroker()
  .Build()
  .WithOpenApiDocumentation()
  .WithAuth();

ApiVersionSet set = app
  .NewApiVersionSet()
  .HasApiVersion(new(1))
  .ReportApiVersions()
  .Build();

app
  .WithSwagger()
  .MapGroup("/api/v{version:apiVersion}")
  .WithApiVersionSet(set)
  .RegistryRoutes();

app
  .Run();