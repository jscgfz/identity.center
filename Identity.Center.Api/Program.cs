using Identity.Center.Application.Extensions;
using Identity.Center.Infrastructure.Extensions;
using Identity.Center.Persistence.Extensions;

WebApplicationBuilder builder = WebApplication
  .CreateBuilder(args)
  .WithIdentityPersistence()
  .WithAuth()
  .WithCache()
  .WithResultExtensions();

// Add services to the container.

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

string[] summaries =
[
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
];

app.MapGet("/weatherforecast", () =>
{
  IEnumerable<WeatherForecast> forecast = Enumerable.Range(1, 5).Select(index =>
      new WeatherForecast
      (
          DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
          Random.Shared.Next(-20, 55),
          summaries[Random.Shared.Next(summaries.Length)]
      ))
      .ToArray();
  return forecast;
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
