using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Builds;

internal sealed class AppConfiguration : IEntityTypeConfiguration<App>
{
  public void Configure(EntityTypeBuilder<App> builder)
  {
    builder
      .ToTable("apps", IdentitySchemas.Builds);
    builder
      .Property(row => row.Index)
      .HasColumnName("index")
      .UseIdentityColumn();
    builder
      .Property(row => row.Prefix)
      .HasColumnName("prefix");
    builder
      .Property(row => row.DomainName)
      .HasColumnName("domain_name");
    builder
      .HasIndex(row => row.Index)
      .IsUnique();
    builder
      .HasIndex(row => row.Prefix)
      .IsUnique();
    builder
      .HasData([
        new App {
          Id = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Index = 1,
          Name = "Identity",
          Prefix = "sid",
          Description = "Servicio de gestión de identidad",
        },
        new App {
          Id = Guid.Parse("2cab2232-72a0-f011-81de-00505682eca9"),
          Index = 2,
          Name = "Atenea Iris",
          Prefix = "ais",
          Description = "Servicios de atención telefónica centralizados",
        },
        new App {
          Id = Guid.Parse("befbac57-72a0-f011-81de-00505682eca9"),
          Index = 3,
          Name = "Atenea Asisya",
          Prefix = "aay",
          Description = "Servicios de atención telefónica centralizados",
        },
        new App {
          Id = Guid.Parse("085e4fa8-72a0-f011-81de-00505682eca9"),
          Index = 4,
          Name = "Atenea Promotec",
          Prefix = "apt",
          Description = "Servicios de atención telefónica para Promotec",
        },
        new App {
          Id = Guid.Parse("99d8850b-73a0-f011-81de-00505682eca9"),
          Index = 5,
          Name = "Central Asterisk",
          Prefix = "ast",
          Description = "Servicios de control y monitoreo de telefonía",
        },
        new App {
          Id = Guid.Parse("c20bfd03-77a0-f011-81de-00505682eca9"),
          Index = 6,
          Name = "Finanzauto Web Admin",
          Prefix = "fzw",
          Description = "Administrador de credenciales de la web de finanzauto",
        },
        new App {
          Id = Guid.Parse("7b2c9ab9-77a0-f011-81de-00505682eca9"),
          Index = 7,
          Name = "Atenea Carfiao",
          Prefix = "acf",
          Description = "Servicios de atención telefónica para Carfiao",
        },
      ]);
  }
}
