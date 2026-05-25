using System.Reflection;
using Identity.Center.Application.Common.Options;
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
      .AddPooledDbContextFactory<IdentityContext>((provider, options) =>
      {
        EnvironmentOptions? envOptions = provider.GetRequiredService<IConfiguration>()
          .GetSection(nameof(EnvironmentOptions))
          .Get<EnvironmentOptions>();

        options.UseSqlServer(
          provider.GetRequiredService<IConfiguration>().GetConnectionString(nameof(IdentityContext)),
          sql =>
          {
            sql.MigrationsHistoryTable("migrations", IdentitySchemas.Builds);
            sql.MigrationsAssembly(Assembly.GetExecutingAssembly());
            sql.CommandTimeout(30);
            sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
          }
        );
        if (envOptions != null && envOptions.IsDevEnvironment(ref provider))
        {
          options.EnableDetailedErrors();
          options.EnableSensitiveDataLogging();
        }
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
      });

    builder
      .Services
      .AddScoped(sp => sp.GetRequiredService<IDbContextFactory<IdentityContext>>().CreateDbContext());
    builder
      .Services
      .AddScoped<DbContext>(sp => sp.GetRequiredService<IdentityContext>());

    return builder;
  }
}
