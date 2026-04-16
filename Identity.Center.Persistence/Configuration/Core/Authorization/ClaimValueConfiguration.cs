using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Authorization;

internal sealed class ClaimValueConfiguration : IEntityTypeConfiguration<ClaimValue>
{
  public void Configure(EntityTypeBuilder<ClaimValue> builder)
  {
    builder
      .ToTable("claims", IdentitySchemas.Authorization);
    builder
      .Property(row => row.ActionId)
      .HasColumnName("action_id");
    builder
      .Property(row => row.GroupId)
      .HasColumnName("group_id");
    builder
      .HasOne(row => row.Action)
      .WithMany(row => row.Claims)
      .HasForeignKey(row => row.ActionId)
      .OnDelete(DeleteBehavior.NoAction);
    builder
      .HasOne(row => row.Group)
      .WithMany(row => row.Claims)
      .HasForeignKey(row => row.GroupId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
