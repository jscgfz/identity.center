using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Authentication;

internal sealed class SingleCredentialConfiguration : IEntityTypeConfiguration<SingleCredential>
{
  public void Configure(EntityTypeBuilder<SingleCredential> builder)
  {
    builder
      .ToTable("single_credentials", IdentitySchemas.Authentication);
    builder
      .Property(row => row.UserId)
      .HasColumnName("user_id");
    builder
      .Property(row => row.AppId)
      .HasColumnName("app_id");
    builder
      .Property(row => row.Username)
      .HasColumnName("username");
    builder
      .Property(row => row.Hash)
      .HasColumnName("hash");
    builder
      .Property(row => row.Salt)
      .HasColumnName("salt");
    builder
      .HasKey(row => new { row.AppId, row.UserId });
    builder
      .HasIndex(row => new { row.AppId, row.Username })
      .IsUnique();
    builder
      .HasOne(row => row.User)
      .WithMany(row => row.SingleCredentials)
      .HasForeignKey(row => row.UserId)
      .OnDelete(DeleteBehavior.NoAction);
    builder
      .HasOne(row => row.App)
      .WithMany(row => row.Credentials)
      .HasForeignKey(row => row.AppId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
