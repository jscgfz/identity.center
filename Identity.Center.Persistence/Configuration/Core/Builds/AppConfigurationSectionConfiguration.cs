using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Builds;

internal sealed class AppConfigurationSectionConfiguration : IEntityTypeConfiguration<AppConfigurationSection>
{
  public void Configure(EntityTypeBuilder<AppConfigurationSection> builder)
  {
    builder
      .ToTable("app_settings", IdentitySchemas.Builds);
    builder
      .Property(row => row.AppId)
      .HasColumnName("app_id");
    builder
      .Property(row => row.Key)
      .HasColumnName("key");
    builder
      .Property(row => row.Value)
      .HasColumnName("value");
    builder
      .HasKey(row => new { row.AppId, row.Key });
    builder
      .HasOne(row => row.App)
      .WithMany(row => row.ConfigurationSections)
      .HasForeignKey(row => row.AppId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
