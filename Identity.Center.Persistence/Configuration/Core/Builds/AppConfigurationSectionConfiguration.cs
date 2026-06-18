using Identity.Center.Application.Common.Options;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Center.Persistence.Configuration.Core.Builds;

internal sealed class AppConfigurationSectionConfiguration : IEntityTypeConfiguration<AppConfigurationSection>
{
  public void Configure(EntityTypeBuilder<AppConfigurationSection> builder)
  {
    builder
      .ToTable("app_settings", IdentitySchemas.Builds);
    builder
      .Property(row => row.AppId)
      .HasColumnName("app_id");
    builder
      .Property(row => row.Key)
      .HasColumnName("key");
    builder
      .Property(row => row.Value)
      .HasColumnName("value");
    builder
      .HasKey(row => new { row.AppId, row.Key });
    builder
      .HasOne(row => row.App)
      .WithMany(row => row.ConfigurationSections)
      .HasForeignKey(row => row.AppId)
      .OnDelete(DeleteBehavior.NoAction);
    builder
      .HasData([
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(MasivianOptions)}:{nameof(MasivianOptions.EmailBaseUrl)}",
          Value = "https://api.masiv.masivian.com"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(MasivianOptions)}:{nameof(MasivianOptions.SmsBaseUrl)}",
          Value = "https://api-sms.masivapp.com"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(MasivianOptions)}:{nameof(MasivianOptions.Username)}",
          Value = "finanzauto_wdh4-"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(MasivianOptions)}:{nameof(MasivianOptions.Password)}",
          Value = "J5X5W_NXJ5"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(MasivianOptions)}:{nameof(MasivianOptions.Sender)}",
          Value = "identity.notifications@seissa.com.co"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(SmtpOptions)}:{nameof(SmtpOptions.Host)}",
          Value = "smtp.gmail.com"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(SmtpOptions)}:{nameof(SmtpOptions.Port)}",
          Value = "587"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(SmtpOptions)}:{nameof(SmtpOptions.Username)}",
          Value = "servidorweb@finanzauto.com.co"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(SmtpOptions)}:{nameof(SmtpOptions.Password)}",
          Value = "vijsrsgsowdgizkc"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(QdControlOptions)}:{nameof(QdControlOptions.BaseUrl)}",
          Value = "http://www.qdatacolombia.com/Services/ServiciosApi/ServiceAutenticacionLDAP"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(ContactTypesOptions)}:{nameof(ContactTypesOptions.CellPhoneExpressions)}:0",
          Value = @"^\+573\d{9}$"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(ContactTypesOptions)}:{nameof(ContactTypesOptions.EmailExpressions)}:1",
          Value = @"^([a-z]+\.[a-z]+@finanzauto\.com\.co)$"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(ContactTypesOptions)}:{nameof(ContactTypesOptions.EmailExpressions)}:2",
          Value = @"^([a-z]+\.[a-z]+@promotec\.com)$"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(ContactTypesOptions)}:{nameof(ContactTypesOptions.EmailExpressions)}:3",
          Value = @"^([a-z]+\.[a-z]+@asisya\.com)$"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(ContactTypesOptions)}:{nameof(ContactTypesOptions.EmailExpressions)}:4",
          Value = @"^([a-z]+\.[a-z]+@carfiao\.com)$"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(AlfrescoOptions)}:{nameof(AlfrescoOptions.BaseUrl)}",
          Value = "http://192.168.50.76:11001/alfresco/api/-default-/public/alfresco/versions/1"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(AlfrescoOptions)}:{nameof(AlfrescoOptions.Username)}",
          Value = "laura.roa"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(AlfrescoOptions)}:{nameof(AlfrescoOptions.Password)}",
          Value = "L0nd0n$.$"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(AlfrescoOptions)}:{nameof(AlfrescoOptions.NodeCollection)}:{nameof(Authorization)}",
          Value = "76a0b619-3b8a-44f5-a947-b3a6a4db8378"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(AlfrescoOptions)}:{nameof(AlfrescoOptions.ValidMimeTypes)}:0",
          Value = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        },
        new AppConfigurationSection() {
          AppId = Guid.Parse("5f9c0e66-e29f-f011-81de-00505682eca9"),
          Key = $"{nameof(AlfrescoOptions)}:{nameof(AlfrescoOptions.ValidMimeTypes)}:1",
          Value = "application/pdf"
        },
      ]);
  }
}
