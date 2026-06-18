using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Center.Persistence.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class RoleRequestControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "change_control",
                schema: "sec",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
                    role_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "pending"),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    authorization_document = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    current_picture = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    request_picture = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
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
                    table.PrimaryKey("PK_change_control", x => x.id);
                    table.ForeignKey(
                        name: "FK_change_control_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "id",
                        principalTable: "roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_change_control_role_id",
                schema: "sec",
                table: "change_control",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "change_control",
                schema: "sec");
        }
    }
}
