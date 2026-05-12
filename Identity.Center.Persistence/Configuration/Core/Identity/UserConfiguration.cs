using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Identity;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder
      .ToTable("users", IdentitySchemas.Identity);
    builder
      .Property(row => row.DocumentType)
      .HasColumnName("document_type")
      .HasMaxLength(5);
    builder
      .Property(row => row.DocumentNumber)
      .HasColumnName("document_number")
      .HasMaxLength(30);
    builder
      .Property(row => row.FirstName)
      .HasColumnName("first_name")
      .HasMaxLength(30);
    builder
      .Property(row => row.SecondName)
      .HasColumnName("second_name")
      .HasMaxLength(30);
    builder
      .Property(row => row.FirstLastName)
      .HasColumnName("first_lastname")
      .HasMaxLength(30);
    builder
      .Property(row => row.SecondLastName)
      .HasColumnName("second_lastname")
      .HasMaxLength(30);
    builder
      .Property(row => row.MfaSignature)
      .HasColumnName("mfa_signature");
    builder
      .HasIndex(row => new { row.DocumentType, row.DocumentNumber })
      .IsUnique();
  }
}
