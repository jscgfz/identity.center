using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Center.Persistence.Data.Core.Migrations
{
  /// <inheritdoc />
  public partial class ApiKeyAllowedOrigins : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "allowed_origins",
          schema: "sec",
          columns: table => new
          {
            id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
            api_key_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            origin = table.Column<string>(type: "nvarchar(450)", nullable: false),
            created_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "getutcdate()"),
            created_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
            last_modified_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            last_modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            deleted_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            deleted_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_allowed_origins", x => x.id);
            table.ForeignKey(
                      name: "FK_allowed_origins_api_keys_api_key_id",
                      column: x => x.api_key_id,
                      principalSchema: "auth",
                      principalTable: "api_keys",
                      principalColumn: "id");
          });

      migrationBuilder.CreateIndex(
          name: "IX_allowed_origins_api_key_id_origin",
          schema: "sec",
          table: "allowed_origins",
          columns: new[] { "api_key_id", "origin" },
          unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "allowed_origins",
          schema: "sec");
    }
  }
}
