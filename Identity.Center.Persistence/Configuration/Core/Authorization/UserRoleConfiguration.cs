using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Authorization;

internal sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
  public void Configure(EntityTypeBuilder<UserRole> builder)
  {
    builder
      .ToTable("users_roles", IdentitySchemas.Authorization);
    builder
      .Property(row => row.UserId)
      .HasColumnName("user_id");
    builder
      .Property(row => row.RoleId)
      .HasColumnName("role_id");
    builder
      .HasKey(row => new { row.UserId, row.RoleId });
    builder
      .HasOne(row => row.User)
      .WithMany(row => row.Roles)
      .HasForeignKey(r => r.UserId)
      .OnDelete(DeleteBehavior.NoAction);
    builder
      .HasOne(row => row.Role)
      .WithMany(row => row.Users)
      .HasForeignKey(r => r.RoleId)
      .OnDelete(DeleteBehavior.NoAction);
  }
}
