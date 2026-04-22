using Asp.Versioning.ApiExplorer;
using Identity.Center.Api.Extensions;
using Identity.Center.Application.Extensions;
using Identity.Center.Infrastructure.Configuration.Authentication;
using Identity.Center.Infrastructure.Extensions;
using Identity.Center.Persistence.Extensions;
using Microsoft.AspNetCore.Authorization;

WebApplicationBuilder builder = WebApplication
  .CreateBuilder(args)
  .WithIdentityPersistence()
  .WithAuth()
  .WithCache()
  .WithVersioning()
  .WithOpenApiDocumentation("v1")
  .WithResultExtensions();

// Add services to the container.

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi("/openapi/{documentName}/openapidoc.json");
app.UseSwaggerUI(options =>
{
  IEnumerable<ApiVersionDescription> versions = app.DescribeApiVersions();
  foreach (ApiVersionDescription version in versions)
    options.SwaggerEndpoint($"/openapi/{version.GroupName}/openapidoc.json", version.GroupName);
});

string[] summaries =
[
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
];

app.MapGet("/weatherforecast", () =>
{
  IEnumerable<WeatherForecast> forecast = [.. Enumerable.Range(1, 5).Select(index =>
      new WeatherForecast
      (
          DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
          Random.Shared.Next(-20, 55),
          summaries[Random.Shared.Next(summaries.Length)]
      ))];
  return forecast;
})
  .RequireAuthorization()
  .Produces(StatusCodes.Status401Unauthorized);

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
