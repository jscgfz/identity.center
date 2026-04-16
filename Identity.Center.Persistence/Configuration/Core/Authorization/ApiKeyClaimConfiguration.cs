using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Authorization;

internal sealed class ApiKeyClaimConfiguration : IEntityTypeConfiguration<ApiKeyClaim>
{
  public void Configure(EntityTypeBuilder<ApiKeyClaim> builder)
  {
    builder
      .ToTable("api_keys_claims", IdentitySchemas.Authorization);
    builder
      .Property(row => row.ApiKeyId)
      .HasColumnName("api_key_id");
    builder
      .Property(row => row.ClaimId)
      .HasColumnName("claim_id");
    builder
      .HasKey(row => new { row.ApiKeyId, row.ClaimId });
    builder
      .HasOne(row => row.ApiKey)
      .WithMany(row => row.Claims)
      .HasForeignKey(row => row.ApiKeyId)
      .OnDelete(DeleteBehavior.NoAction);
    builder
      .HasOne(row => row.Claim)
      .WithMany(row => row.ApiKeys)
      .HasForeignKey(row => row.ClaimId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
