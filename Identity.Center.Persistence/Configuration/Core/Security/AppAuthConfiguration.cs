using System.Security.Cryptography;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Security;

internal sealed class AppAuthConfiguration : IEntityTypeConfiguration<AppAuth>
{
  public void Configure(EntityTypeBuilder<AppAuth> builder)
  {
    builder
      .ToTable("app_auth", IdentitySchemas.Security);
    builder
      .Property(row => row.AppId)
      .HasColumnName("app_id");
    builder
      .Property(row => row.SignatureKey)
      .HasColumnName("signature_key");
    builder
      .Property(row => row.TwoFactorEnabled)
      .HasDefaultValue(false)
      .HasColumnName("two_factor_enabled");
    builder
      .Property(row => row.ExpirationTime)
      .HasDefaultValue(TimeSpan.FromHours(1))
      .HasColumnName("expiration_time");
    builder
      .Property(row => row.RefreshTime)
      .HasDefaultValue(TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(30)))
      .HasColumnName("refresh_time");
    builder
      .HasKey(row => row.AppId);
    builder
      .HasIndex(row => row.SignatureKey)
      .IsUnique();
    builder
      .HasOne(row => row.App)
      .WithOne(row => row.Auth)
      .HasForeignKey<AppAuth>(row => row.AppId)
      .OnDelete(DeleteBehavior.NoAction); builder
      .HasData([
        new AppAuth {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          SignatureKey = [ 19, 161, 15, 77, 197, 84, 141, 150, 231, 18, 90, 115, 18, 1, 45, 18, 17, 246, 235, 91, 213, 252, 90, 154, 46, 179, 179, 170, 31, 83, 36, 100 ]
        },
        new AppAuth {
          AppId = Guid.Parse("2cab2232-72a0-f011-81de-00505682eca9"),
          SignatureKey = [ 249, 18, 34, 98, 74, 76, 224, 211, 201, 247, 70, 92, 58, 65, 182, 217, 181, 96, 181, 61, 99, 191, 241, 233, 207, 26, 209, 91, 227, 46, 15, 122 ]
        },
        new AppAuth {
          AppId = Guid.Parse("befbac57-72a0-f011-81de-00505682eca9"),
          SignatureKey = [ 100, 80, 12, 96, 214, 214, 199, 10, 172, 99, 43, 207, 43, 222, 15, 139, 167, 130, 34, 38, 139, 56, 253, 196, 138, 220, 102, 135, 182, 8, 128, 63 ]
        },
        new AppAuth {
          AppId = Guid.Parse("085e4fa8-72a0-f011-81de-00505682eca9"),
          SignatureKey = [ 91, 226, 250, 169, 173, 40, 65, 160, 238, 110, 230, 102, 25, 36, 14, 179, 195, 15, 218, 62, 41, 138, 146, 88, 51, 114, 195, 34, 182, 196, 153, 99 ]
        },
        new AppAuth {
          AppId = Guid.Parse("99d8850b-73a0-f011-81de-00505682eca9"),
          SignatureKey = [ 131, 181, 168, 162, 251, 106, 215, 25, 15, 146, 239, 52, 140, 147, 47, 226, 181, 88, 117, 23, 69, 214, 234, 176, 252, 159, 247, 115, 196, 206, 54, 141 ]
        },
        new AppAuth {
          AppId = Guid.Parse("c20bfd03-77a0-f011-81de-00505682eca9"),
          SignatureKey = [ 225, 99, 122, 255, 85, 23, 96, 166, 23, 30, 120, 30, 179, 180, 171, 182, 255, 119, 18, 248, 132, 204, 37, 232, 186, 199, 63, 22, 46, 160, 16, 7 ]
        },
        new AppAuth {
          AppId = Guid.Parse("7b2c9ab9-77a0-f011-81de-00505682eca9"),
          SignatureKey = [ 101, 206, 146, 19, 58, 149, 193, 222, 177, 24, 199, 139, 66, 42, 16, 125, 221, 209, 254, 155, 118, 208, 165, 154, 67, 252, 3, 229, 228, 15, 185, 62 ]
        },
      ]);
  }
}
