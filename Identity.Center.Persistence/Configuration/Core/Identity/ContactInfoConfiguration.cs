using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Identity;

internal sealed class ContactInfoConfiguration : IEntityTypeConfiguration<ContactInfo>
{
  public void Configure(EntityTypeBuilder<ContactInfo> builder)
  {
    builder
      .ToTable("contact_info", IdentitySchemas.Identity);
    builder
      .Property(row => row.UserId)
      .HasColumnName("user_id");
    builder
      .Property(row => row.ContactTypeId)
      .HasColumnName("contact_type_id");
    builder
      .Property(row => row.Value)
      .HasColumnName("value");
    builder
      .Property(row => row.Salt)
      .HasColumnName("salt");
    builder
      .Property(row => row.Confirmed)
      .HasColumnName("confirmed");
    builder
      .HasIndex(row => new { row.UserId, row.ContactTypeId, row.Value })
      .IsUnique();
    builder
      .HasOne(row => row.User)
      .WithMany(row => row.ContactInfo)
      .HasForeignKey(row => row.UserId)
      .OnDelete(DeleteBehavior.NoAction);
    builder
      .HasOne(row => row.ContactType)
      .WithMany(row => row.ContactInfo)
      .HasForeignKey(row => row.ContactTypeId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
