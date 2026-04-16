using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Authorization;

internal sealed class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
  public void Configure(EntityTypeBuilder<RoleClaim> builder)
  {
    builder
      .ToTable("roles_claims", IdentitySchemas.Authorization);
    builder
      .Property(row => row.RoleId)
      .HasColumnName("role_id");
    builder
      .Property(row => row.ClaimId)
      .HasColumnName("claim_id");
    builder
      .HasKey(row => new { row.RoleId,  row.ClaimId });
    builder
      .HasOne(row => row.Role)
      .WithMany(row => row.Claims)
      .HasForeignKey(row => row.RoleId)
      .OnDelete(DeleteBehavior.NoAction);
    builder
      .HasOne(row => row.Claim)
      .WithMany(row => row.Roles)
      .HasForeignKey(row => row.ClaimId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
