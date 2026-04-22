using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Builder;
using Identity.Center.Api.Extensions;
using Identity.Center.Application.Extensions;
using Identity.Center.Infrastructure.Configuration.Authentication;
using Identity.Center.Infrastructure.Extensions;
using Identity.Center.Persistence.Extensions;
using Microsoft.AspNetCore.Authorization;

WebApplication app = WebApplication
  .CreateBuilder(args)
  .WithIdentityPersistence()
  .WithAuth()
  .WithCache()
  .WithVersioning()
  .WithOpenApiDocumentation("v1")
  .WithResultExtensions()
  .Build()
  .WithAuth()
  .WithOpenApiDocumentation();

ApiVersionSet set = app
  .NewApiVersionSet()
  .HasApiVersion(new(1))
  .ReportApiVersions()
  .Build();

app
  .MapGroup("/api/v{version:apiVersion}")
  .WithApiVersionSet(set)
  .RegistryRoutes();

app
  .WithSwagger()
  .Run();