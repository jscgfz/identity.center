using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Security;

internal class AllowedOriginsConfiguration : IEntityTypeConfiguration<AllowedOrigin>
{
  public void Configure(EntityTypeBuilder<AllowedOrigin> builder)
  {
    builder
      .ToTable("allowed_origins", IdentitySchemas.Security);

    builder
      .Property(row => row.ApiKeyId)
      .HasColumnName("api_key_id");

    builder
      .Property(row => row.Origin)
      .HasColumnName("origin");

    builder
      .HasIndex(row => new { row.ApiKeyId, row.Origin })
      .IsUnique();

    builder
      .HasOne(row => row.ApiKey)
      .WithMany(row => row.AllowedOrigins)
      .HasForeignKey(row => row.ApiKeyId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
