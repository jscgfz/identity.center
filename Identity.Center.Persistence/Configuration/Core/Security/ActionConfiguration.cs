using Identity.Center.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Action = Identity.Center.Domain.Entities.Core.Security.Action;

namespace Identity.Center.Persistence.Configuration.Core.Security;

internal sealed class ActionConfiguration : IEntityTypeConfiguration<Action>
{
  public void Configure(EntityTypeBuilder<Action> builder)
  {
    builder
      .ToTable("actions", IdentitySchemas.Security);

    builder
      .HasData([
        new Action { Name = "download", Id = Guid.Parse("6343DA36-3343-F111-81E9-00505682ECA9"), Description = "Descargar" },
        new Action { Name = "create", Id = Guid.Parse("6443DA36-3343-F111-81E9-00505682ECA9"), Description = "Crear" },
        new Action { Name = "view", Id = Guid.Parse("6543DA36-3343-F111-81E9-00505682ECA9"), Description = "Ver" },
        new Action { Name = "update", Id = Guid.Parse("6643DA36-3343-F111-81E9-00505682ECA9"), Description = "Modificar/Actualizar" },
        new Action { Name = "upload", Id = Guid.Parse("6743DA36-3343-F111-81E9-00505682ECA9"), Description = "Cargar" },
        new Action { Name = "delete", Id = Guid.Parse("6843DA36-3343-F111-81E9-00505682ECA9"), Description = "Eliminar" },
      ]);
  }
}
