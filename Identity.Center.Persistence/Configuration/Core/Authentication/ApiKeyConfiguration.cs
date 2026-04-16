using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Authentication;

internal sealed class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
  public void Configure(EntityTypeBuilder<ApiKey> builder)
  {
    builder
      .ToTable("api_keys", IdentitySchemas.Authentication);
    builder
      .Property(row => row.AppId)
      .HasColumnName("app_id");
    builder
      .Property(row => row.Hash)
      .HasColumnName("hash");
    builder
      .Property(row => row.Salt)
      .HasColumnName("salt");
    builder
      .Property(row => row.Root)
      .HasColumnName("root")
      .HasDefaultValue(false);
    builder
      .HasOne(row => row.App)
      .WithMany(row => row.ApiKeys)
      .HasForeignKey(row => row.AppId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
