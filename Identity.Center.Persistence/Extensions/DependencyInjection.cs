using System.Reflection;
using Identity.Center.Domain.Constants;
using Identity.Center.Persistence.Data.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Persistence.Extensions;

public static class DependencyInjection
{
  public static WebApplicationBuilder WithIdentityPersistence(this WebApplicationBuilder builder)
  {
    builder
      .Services
      .AddDbContext<IdentityContext>((provider, options) =>
      {
        options.UseSqlServer(
          provider.GetRequiredService<IConfiguration>().GetConnectionString(nameof(IdentityContext)),
          sql =>
          {
            sql.MigrationsHistoryTable("migrations", IdentitySchemas.Builds);
            sql.MigrationsAssembly(Assembly.GetExecutingAssembly());
            sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
          }
        );
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
      });
    return builder;
  }
}
