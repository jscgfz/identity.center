using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Identity;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
  public void Configure(EntityTypeBuilder<Role> builder)
  {
    builder
      .ToTable("roles", IdentitySchemas.Identity);
    builder
      .Property(row => row.AppId)
      .HasColumnName("app_id");
    builder
      .Property(row => row.Name)
      .HasColumnName("name");
    builder
      .Property(row => row.Description)
      .HasColumnName("description");
    builder
      .Property(row => row.DomainName)
      .HasColumnName("domain_name");
    builder
      .Property(row => row.ActiveDirectoryMandatory)
      .HasColumnName("ad_mandatory");
    builder
      .HasIndex(row => new { row.Name, row.AppId })
      .IsUnique();
    builder
      .HasIndex(row => row.DomainName)
      .IsUnique();
    builder
      .HasOne(row => row.App)
      .WithMany(row => row.Roles)
      .HasForeignKey(row => row.AppId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
