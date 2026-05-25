using Identity.Center.Persistence.Configuration.Core.Authentication;
using Identity.Center.Persistence.Configuration.Core.Authorization;
using Identity.Center.Persistence.Configuration.Core.Builds;
using Identity.Center.Persistence.Configuration.Core.Identity;
using Identity.Center.Persistence.Configuration.Core.Security;
using Identity.Center.Persistence.Conventions;
using Microsoft.EntityFrameworkCore;

namespace Identity.Center.Persistence.Extensions;

internal static class DbContextExtensions
{
  private static ModelBuilder WithAuthSchema(this ModelBuilder modelBuilder)
    => modelBuilder
      .ApplyConfiguration(new ApiKeyConfiguration())
      .ApplyConfiguration(new DomainCredentialConfiguration())
      .ApplyConfiguration(new SingleCredentialConfiguration())
      .ApplyConfiguration(new ApiKeyClaimConfiguration())
      .ApplyConfiguration(new ClaimValueConfiguration())
      .ApplyConfiguration(new RoleClaimConfiguration())
      .ApplyConfiguration(new UserRoleConfiguration());

  private static ModelBuilder WithBuildSchema(this ModelBuilder modelBuilder)
    => modelBuilder
      .ApplyConfiguration(new AppAllowedCredentialConfiguration())
      .ApplyConfiguration(new AppConfiguration())
      .ApplyConfiguration(new AppConfigurationSectionConfiguration())
      .ApplyConfiguration(new CredentialTypeConfiguration())
      .ApplyConfiguration(new HealtCheckConfiguration());

  private static ModelBuilder WithIdSchema(this ModelBuilder modelBuilder)
    => modelBuilder
      .ApplyConfiguration(new ContactInfoConfiguration())
      .ApplyConfiguration(new RoleConfiguration())
      .ApplyConfiguration(new UserConfiguration());

  private static ModelBuilder WithSecSchema(this ModelBuilder modelBuilder)
    => modelBuilder
      .ApplyConfiguration(new ActionConfiguration())
      .ApplyConfiguration(new AllowedOriginsConfiguration())
      .ApplyConfiguration(new AppAuthConfiguration())
      .ApplyConfiguration(new ContactTypeConfiguration())
      .ApplyConfiguration(new GroupConfiguration());

  public static ModelBuilder WithSchemas(this ModelBuilder modelBuilder)
    => modelBuilder
      .WithAuthSchema()
      .WithBuildSchema()
      .WithIdSchema()
      .WithSecSchema();

  public static ModelConfigurationBuilder WithIdentityConventions(this ModelConfigurationBuilder builder)
  {
    builder.Conventions.Add(_ => new CreatedEntityFieldsConvention());
    builder.Conventions.Add(_ => new KeyedEntityConvention());
    builder.Conventions.Add(_ => new LastModifiedEntityFieldsConvention());
    builder.Conventions.Add(_ => new MasterFieldsConventions());
    builder.Conventions.Add(_ => new SoftDeletedEntityFieldsConvention());
    return builder;
  }
}
