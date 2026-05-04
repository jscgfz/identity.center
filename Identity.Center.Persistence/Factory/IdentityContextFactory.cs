using System.Reflection;
using Identity.Center.Domain.Constants;
using Identity.Center.Persistence.Data.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Identity.Center.Persistence.Factory;

public class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
{
  public IdentityContext CreateDbContext(string[] args)
  {
    // Construimos la configuración manualmente
    IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        // Opcional: permite leer secretos de usuario o variables de entorno
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
        .Build();

    DbContextOptionsBuilder<IdentityContext> options = new DbContextOptionsBuilder<IdentityContext>();
    string? connectionString = configuration.GetConnectionString(nameof(IdentityContext));

    options.UseSqlServer(
          connectionString,
          sql =>
          {
            sql.MigrationsHistoryTable("migrations", IdentitySchemas.Builds);
            sql.MigrationsAssembly(Assembly.GetExecutingAssembly());
            sql.CommandTimeout(30);
            sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
          }
        );
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

    return new IdentityContext(options.Options);
  }
}
