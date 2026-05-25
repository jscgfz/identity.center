using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Center.Persistence.Data.Core.Migrations
{
  /// <inheritdoc />
  public partial class QdOptions : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.InsertData(
          schema: "build",
          table: "app_settings",
          columns: new[] { "app_id", "key", "deleted_at_utc", "deleted_by", "last_modified_at_utc", "last_modified_by", "value" },
          values: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "QdControlOptions:BaseUrl", null, null, null, null, "http://www.qdatacolombia.com/Services/ServiciosApi/ServiceAutenticacionLDAP" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DeleteData(
          schema: "build",
          table: "app_settings",
          keyColumns: new[] { "app_id", "key" },
          keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "QdControlOptions:BaseUrl" });
    }
  }
}
