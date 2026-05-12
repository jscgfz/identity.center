using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Security;

internal sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
  public void Configure(EntityTypeBuilder<Group> builder)
  {
    builder
      .ToTable("groups", IdentitySchemas.Security);
    builder
      .HasData([
        new Group { Name = "users", Id = Guid.Parse("16842A5F-3943-F111-81E9-00505682ECA9"), Description = "Usuarios" },
        new Group { Name = "groups", Id = Guid.Parse("17842A5F-3943-F111-81E9-00505682ECA9"), Description = "Grupos" },
        new Group { Name = "healtchecks", Id = Guid.Parse("18842A5F-3943-F111-81E9-00505682ECA9"), Description = "HealtChecks" },
        new Group { Name = "apps", Id = Guid.Parse("19842A5F-3943-F111-81E9-00505682ECA9"), Description = "Aplicaciones" },
        new Group { Name = "contacttypes", Id = Guid.Parse("1A842A5F-3943-F111-81E9-00505682ECA9"), Description = "Tipos de contacto" },
        new Group { Name = "settings", Id = Guid.Parse("1B842A5F-3943-F111-81E9-00505682ECA9"), Description = "Configuraciones" },
        new Group { Name = "claims", Id = Guid.Parse("1C842A5F-3943-F111-81E9-00505682ECA9"), Description = "Permisos" },
        new Group { Name = "actions", Id = Guid.Parse("1D842A5F-3943-F111-81E9-00505682ECA9"), Description = "Acciones" },
        new Group { Name = "credentialtypes", Id = Guid.Parse("1E842A5F-3943-F111-81E9-00505682ECA9"), Description = "Tipos de credenciales" },
        new Group { Name = "apikeys", Id = Guid.Parse("9DAFF3D6-BA48-F111-81E9-00505682ECA9"), Description = "Api keys" },
        new Group { Name = "credentials", Id = Guid.Parse("C93E61D8-9949-F111-81E9-00505682ECA9"), Description = "Credenciales de usuario" },
      ]);
  }
}
