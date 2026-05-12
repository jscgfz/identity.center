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
    IConfigurationRoot configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddEnvironmentVariables()
      .AddJsonFile("appsettings.json")
      .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
      .Build();

    DbContextOptionsBuilder<IdentityContext> options = new DbContextOptionsBuilder<IdentityContext>();
    string connectionString = configuration.GetConnectionString(nameof(IdentityContext)) ??
      "Server=192.168.40.106; Database=Identity.Center.Stores; User=Applications; Password=uKOn8yR177N57q9; TrustServerCertificate=true;Encrypt=False;MultipleActiveResultSets=True";

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
