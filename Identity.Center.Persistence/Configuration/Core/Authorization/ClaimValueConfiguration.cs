using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Authorization;

internal sealed class ClaimValueConfiguration : IEntityTypeConfiguration<ClaimValue>
{
  public void Configure(EntityTypeBuilder<ClaimValue> builder)
  {
    builder
      .ToTable("claims", IdentitySchemas.Authorization);
    builder
      .Property(row => row.ActionId)
      .HasColumnName("action_id");
    builder
      .Property(row => row.GroupId)
      .HasColumnName("group_id");
    builder
      .HasOne(row => row.Action)
      .WithMany(row => row.Claims)
      .HasForeignKey(row => row.ActionId)
      .OnDelete(DeleteBehavior.NoAction);
    builder
      .HasOne(row => row.Group)
      .WithMany(row => row.Claims)
      .HasForeignKey(row => row.GroupId)
      .OnDelete(DeleteBehavior.NoAction);
    builder
      .HasData([
        new ClaimValue { ActionId = Guid.Parse("6443DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1D842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("2950CF98-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6843DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1D842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("2A50CF98-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6643DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1D842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("2B50CF98-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6543DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1D842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("2C50CF98-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6443DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("19842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("2D50CF98-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6843DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("19842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("2E50CF98-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6743DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("19842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("2F50CF98-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6543DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("19842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("3050CF98-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6443DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1C842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("A554CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6843DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1C842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("A654CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6643DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1C842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("A754CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6543DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1C842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("A854CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6543DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1A842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("A954CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6443DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1E842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("AA54CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6843DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1E842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("AB54CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6643DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1E842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("AC54CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6543DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1E842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("AD54CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6443DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("17842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("AE54CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6843DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("17842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("AF54CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6643DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("17842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("B054CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6543DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("17842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("B154CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6443DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("18842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("B254CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6843DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("18842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("B354CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6643DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("18842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("B454CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6543DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("18842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("B554CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6443DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1B842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("B654CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6843DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1B842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("B754CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6643DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1B842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("B854CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6543DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("1B842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("B954CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6443DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("16842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("BA54CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6843DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("16842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("BB54CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6643DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("16842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("BC54CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6543DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("16842A5F-3943-F111-81E9-00505682ECA9"), Id = Guid.Parse("BD54CC9E-0E44-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6543DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("9DAFF3D6-BA48-F111-81E9-00505682ECA9"), Id = Guid.Parse("66CA6FE3-BA48-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6443DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("9DAFF3D6-BA48-F111-81E9-00505682ECA9"), Id = Guid.Parse("67CA6FE3-BA48-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6643DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("9DAFF3D6-BA48-F111-81E9-00505682ECA9"), Id = Guid.Parse("68CA6FE3-BA48-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6843DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("9DAFF3D6-BA48-F111-81E9-00505682ECA9"), Id = Guid.Parse("69CA6FE3-BA48-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6543DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("C93E61D8-9949-F111-81E9-00505682ECA9"), Id = Guid.Parse("51E4990E-9A49-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6443DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("C93E61D8-9949-F111-81E9-00505682ECA9"), Id = Guid.Parse("52E4990E-9A49-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6643DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("C93E61D8-9949-F111-81E9-00505682ECA9"), Id = Guid.Parse("54E4990E-9A49-F111-81E9-00505682ECA9") },
        new ClaimValue { ActionId = Guid.Parse("6843DA36-3343-F111-81E9-00505682ECA9"), GroupId = Guid.Parse("C93E61D8-9949-F111-81E9-00505682ECA9"), Id = Guid.Parse("57E4990E-9A49-F111-81E9-00505682ECA9") },
      ]);
  }
}
