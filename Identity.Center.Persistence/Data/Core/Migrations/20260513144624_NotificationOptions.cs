using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Identity.Center.Persistence.Data.Core.Migrations
{
  /// <inheritdoc />
  public partial class NotificationOptions : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.InsertData(
          schema: "build",
          table: "app_settings",
          columns: new[] { "app_id", "key", "deleted_at_utc", "deleted_by", "last_modified_at_utc", "last_modified_by", "value" },
          values: new object[,]
          {
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "MasivianOptions:EmailBaseUrl", null, null, null, null, "https://api.masiv.masivian.com" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "MasivianOptions:Password", null, null, null, null, "J5X5W_NXJ5" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "MasivianOptions:Sender", null, null, null, null, "identity.notifications@seissa.com.co" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "MasivianOptions:SmsBaseUrl", null, null, null, null, "https://api-sms.masivapp.com" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "MasivianOptions:Username", null, null, null, null, "finanzauto_wdh4-" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "SmtpOptions:Host", null, null, null, null, "smtp.gmail.com" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "SmtpOptions:Password", null, null, null, null, "vijsrsgsowdgizkc" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "SmtpOptions:Port", null, null, null, null, "587" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "SmtpOptions:Username", null, null, null, null, "servidorweb@finanzauto.com.co" }
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DeleteData(
          schema: "build",
          table: "app_settings",
          keyColumns: new[] { "app_id", "key" },
          keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "MasivianOptions:EmailBaseUrl" });

      migrationBuilder.DeleteData(
          schema: "build",
          table: "app_settings",
          keyColumns: new[] { "app_id", "key" },
          keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "MasivianOptions:Password" });

      migrationBuilder.DeleteData(
          schema: "build",
          table: "app_settings",
          keyColumns: new[] { "app_id", "key" },
          keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "MasivianOptions:Sender" });

      migrationBuilder.DeleteData(
          schema: "build",
          table: "app_settings",
          keyColumns: new[] { "app_id", "key" },
          keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "MasivianOptions:SmsBaseUrl" });

      migrationBuilder.DeleteData(
          schema: "build",
          table: "app_settings",
          keyColumns: new[] { "app_id", "key" },
          keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "MasivianOptions:Username" });

      migrationBuilder.DeleteData(
          schema: "build",
          table: "app_settings",
          keyColumns: new[] { "app_id", "key" },
          keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "SmtpOptions:Host" });

      migrationBuilder.DeleteData(
          schema: "build",
          table: "app_settings",
          keyColumns: new[] { "app_id", "key" },
          keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "SmtpOptions:Password" });

      migrationBuilder.DeleteData(
          schema: "build",
          table: "app_settings",
          keyColumns: new[] { "app_id", "key" },
          keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "SmtpOptions:Port" });

      migrationBuilder.DeleteData(
          schema: "build",
          table: "app_settings",
          keyColumns: new[] { "app_id", "key" },
          keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "SmtpOptions:Username" });
    }
  }
}
