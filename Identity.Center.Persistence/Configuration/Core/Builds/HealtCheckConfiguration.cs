using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Enums;
using Identity.Center.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Builds;

internal sealed class HealtCheckConfiguration : IEntityTypeConfiguration<HealtCheck>
{
  public void Configure(EntityTypeBuilder<HealtCheck> builder)
  {
    builder
      .ToTable("healtchecks", IdentitySchemas.Builds);
    builder
      .Property(row => row.AppId)
      .HasColumnName("app_id");
    builder
      .Property(row => row.Name)
      .HasColumnName("name");
    builder
      .Property(row => row.HealtCheckType)
      .HasColumnName("healtcheck_type")
      .HasConversion(IdentityValueConverters.EnumJson<HealtCheckTypes>());
    builder
      .Property(row => row.Arguments)
      .HasColumnName("arguments")
      .HasDefaultValue(IdentityDefaultValues.EmptyJson)
      .HasConversion(IdentityValueConverters.JsonBytes);
    builder
      .HasOne(row => row.App)
      .WithMany(row => row.HealtChecks)
      .HasForeignKey(row => row.AppId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
