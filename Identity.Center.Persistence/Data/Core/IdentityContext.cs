using Identity.Center.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Persistence.Data.Core;

// dotnet ef migrations add {} -c IdentityContext -s ./Identity.Center.Api -p ./Identity.Center.Persistence -o Data/Core/Migrations -v
public sealed class IdentityContext(DbContextOptions options) : DbContext(options)
{
  protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
  {
    base.ConfigureConventions(
      configurationBuilder.WithIdentityConventions()
    );
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(
      modelBuilder.WithSchemas()
    );
  }
}
