using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Authorization;

internal sealed class RouteClaimConfiguration : IEntityTypeConfiguration<RouteClaim>
{
  public void Configure(EntityTypeBuilder<RouteClaim> builder)
  {
    builder
      .ToTable("routes_claims", IdentitySchemas.Authorization);

    builder
      .HasKey(row => new { row.RouteId, row.ClaimId });

    builder
      .Property(row => row.RouteId)
      .HasColumnName("route_id");

    builder
      .Property(row => row.ClaimId)
      .HasColumnName("claim_id");

    builder
      .HasOne(row => row.Route)
      .WithMany(row => row.Claims)
      .HasForeignKey(row => row.RouteId)
      .OnDelete(DeleteBehavior.NoAction);

    builder
      .HasOne(row => row.Claim)
      .WithMany(row => row.Routes)
      .HasForeignKey(row => row.ClaimId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
