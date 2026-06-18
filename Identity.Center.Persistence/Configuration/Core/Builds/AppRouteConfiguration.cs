using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Builds;

internal sealed class AppRouteConfiguration : IEntityTypeConfiguration<AppRoute>
{
  public void Configure(EntityTypeBuilder<AppRoute> builder)
  {
    builder
      .ToTable("routes", IdentitySchemas.Builds);

    builder
      .HasIndex(row => new
      {
        row.AppId,
        row.Key,
        row.Path
      })
      .IsUnique();

    builder
      .Property(row => row.AppId)
      .HasColumnName("app_id");

    builder
      .Property(row => row.Key)
      .HasColumnName("key");

    builder
      .Property(row => row.Path)
      .HasColumnName("path")
      .HasDefaultValue("/");

    builder
      .Property(row => row.ExcludeNav)
      .HasColumnName("exclude_navigation")
      .HasDefaultValue(true);

    builder
      .Property(row => row.Index)
      .HasColumnName("index")
      .HasDefaultValue(0);

    builder
      .Property(row => row.Icon)
      .HasColumnName("icon");

    builder
      .Property(row => row.ParentRouteId)
      .HasColumnName("parent_route_id");

    builder
      .HasOne(row => row.App)
      .WithMany(row => row.Routes)
      .HasForeignKey(row => row.AppId)
      .OnDelete(DeleteBehavior.NoAction);

    builder
      .HasOne(row => row.ParentRoute)
      .WithMany(row => row.ChildRoutes)
      .HasForeignKey(row => row.ParentRouteId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
