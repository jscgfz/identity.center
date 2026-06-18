using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Domain.Enums;
using Identity.Center.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Security;

internal sealed class ChangeControlConfiguration : IEntityTypeConfiguration<ChangeControl>
{
  public void Configure(EntityTypeBuilder<ChangeControl> builder)
  {
    builder
      .ToTable("change_control", IdentitySchemas.Security);

    builder
      .Property(row => row.RoleId)
      .HasColumnName("role_id");

    builder
      .Property(row => row.Status)
      .HasColumnName("status")
      .HasDefaultValue(ChangeControlStates.Pending)
      .HasConversion(IdentityValueConverters.EnumJson<ChangeControlStates>());

    builder
      .Property(row => row.Reason)
      .HasColumnName("reason");

    builder
      .Property(row => row.AuthorizationDocument)
      .HasColumnName("authorization_document")
      .HasConversion(IdentityValueConverters.JsonBytes);

    builder
      .Property(row => row.CurrentPicture)
      .HasColumnName("current_picture")
      .HasConversion(IdentityValueConverters.JsonBytes);
    
    builder
      .Property(row => row.RequestPicture)
      .HasColumnName("request_picture")
      .HasConversion(IdentityValueConverters.JsonBytes);

    builder
      .HasOne(row => row.Role)
      .WithMany(row => row.History)
      .HasForeignKey(row => row.RoleId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
