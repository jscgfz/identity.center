using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Authentication;

internal sealed class DomainCredentialConfiguration : IEntityTypeConfiguration<DomainCredential>
{
  public void Configure(EntityTypeBuilder<DomainCredential> builder)
  {
    builder
      .ToTable("domain_credentials", IdentitySchemas.Authentication);
    builder
      .Property(row => row.UserId)
      .HasColumnName("user_id");
    builder
      .Property(row => row.CredentialTypeId)
      .HasColumnName("credential_type_id");
    builder
      .HasIndex(row => new { row.CredentialTypeId, row.Username })
      .IsUnique();
    builder
      .HasOne(row => row.User)
      .WithMany(row => row.DomainCredentials)
      .HasForeignKey(row => row.UserId)
      .OnDelete(DeleteBehavior.NoAction);
    builder
      .HasOne(row => row.CredentialType)
      .WithMany(row => row.DomainCredentials)
      .HasForeignKey(row => row.CredentialTypeId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
