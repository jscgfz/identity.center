using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Domain.Enums;
using Identity.Center.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Security;

internal sealed class ContactTypeConfiguration : IEntityTypeConfiguration<ContactType>
{
  public void Configure(EntityTypeBuilder<ContactType> builder)
  {
    builder.ToTable("contact_types", IdentitySchemas.Security);
    builder.HasData([
      new ContactType {
        Id = Guid.Parse("433c5502-cf39-f111-81e9-00505682eca9"),
        Name = "Correo coorporativo",
        ContactTypeKey = ContactTypes.CorporativeMail,
        Description = "Correos internos (correos del workspace)"
      },
      new ContactType {
        Id = Guid.Parse("cc763078-cf39-f111-81e9-00505682eca9"),
        Name = "Correo externo",
        ContactTypeKey = ContactTypes.ExternalMail,
        Description = "Correos externos a la organización"
      },
      new ContactType {
        Id = Guid.Parse("f953c4ae-cf39-f111-81e9-00505682eca9"),
        Name = "Teléfono Celular",
        ContactTypeKey = ContactTypes.Cellphone,
        Description = "Número de celular"
      }
    ]);
    builder
      .Property(row => row.ContactTypeKey)
      .HasColumnName("key")
      .HasConversion(IdentityValueConverters.EnumJson<ContactTypes>());
    builder
      .HasIndex(row => row.ContactTypeKey)
      .IsUnique();
  }
}
