using System.Text.Json;
using Identity.Center.Application.Common.Authentication.Models;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Enums;
using Identity.Center.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Builds;

internal sealed class CredentialTypeConfiguration : IEntityTypeConfiguration<CredentialType>
{
  public void Configure(EntityTypeBuilder<CredentialType> builder)
  {
    builder
      .ToTable("credential_types", IdentitySchemas.Builds);
    builder
      .Property(row => row.AuthType)
      .HasColumnName("auth_method_type")
      .HasConversion(IdentityValueConverters.EnumJson<AuthenticationMethods>());
    builder
      .Property(row => row.Arguments)
      .HasColumnName("arguments")
      .HasDefaultValue(IdentityDefaultValues.EmptyJson)
      .HasConversion(IdentityValueConverters.JsonBytes);
    builder
      .HasData([
        new CredentialType {
          Id = 1,
          Name = "Usuario y contraseña",
          Description = "Ingreso con usuario y contraseña generado por la aplicación",
          AuthType = AuthenticationMethods.Single,
          Arguments = IdentityDefaultValues.EmptyJson
        },
        new CredentialType {
          Id = 2,
          Name = "Finanzauto",
          Description = "Ingreso con usuario de Dominio Finanzauto",
          AuthType = AuthenticationMethods.Quamtum,
          Arguments = JsonSerializer.SerializeToElement(
            new QuamtumAuthAtomicValues("KdNESJeIadQ+U+Q5Qs+8BQ==", "FZCORP"),
            JsonSerializerOptions.Web
          )
        },
        new CredentialType {
          Id = 3,
          Name = "Quantum Data",
          Description = "Ingreso con usuario de Dominio Quantum Data",
          AuthType = AuthenticationMethods.Quamtum,
          Arguments = JsonSerializer.SerializeToElement(
            new QuamtumAuthAtomicValues("KdNESJeIadQ+U+Q5Qs+8BQ==", "QBTA"),
            JsonSerializerOptions.Web
          )
        },
        new CredentialType {
          Id = 4,
          Name = "Promotec",
          Description = "Ingreso con usuario de Dominio Promotec",
          AuthType = AuthenticationMethods.Quamtum,
          Arguments = JsonSerializer.SerializeToElement(
            new QuamtumAuthAtomicValues("KdNESJeIadQ+U+Q5Qs+8BQ==", "PTSEGUROS"),
            JsonSerializerOptions.Web
          )
        },
        new CredentialType {
          Id = 5,
          Name = "Asisya",
          Description = "Ingreso con usuario de Dominio Asisya",
          AuthType = AuthenticationMethods.Quamtum,
          Arguments = JsonSerializer.SerializeToElement(
            new QuamtumAuthAtomicValues("SWJF7E+6Grf63Co9Djy2Jw==", "FZCORP"),
            JsonSerializerOptions.Web
          )
        }
      ]);
  }
}
