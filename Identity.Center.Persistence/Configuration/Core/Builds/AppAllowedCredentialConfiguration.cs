using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Builds;

internal sealed class AppAllowedCredentialConfiguration : IEntityTypeConfiguration<AppAllowedCredential>
{
  public void Configure(EntityTypeBuilder<AppAllowedCredential> builder)
  {
    builder
      .ToTable("apps_allowed_credentials", IdentitySchemas.Builds);
    builder
      .Property(row => row.AppId)
      .HasColumnName("app_id");
    builder
      .Property(row => row.CredentialTypeId)
      .HasColumnName("credential_type_id");
    builder
      .HasKey(row => new { row.AppId, row.CredentialTypeId });
    builder
      .HasOne(row => row.App)
      .WithMany(row => row.AllowedCredentials)
      .HasForeignKey(row => row.AppId)
      .OnDelete(DeleteBehavior.NoAction);
    builder
      .HasOne(row => row.CredentialType)
      .WithMany(row => row.Apps)
      .HasForeignKey(row => row.CredentialTypeId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
