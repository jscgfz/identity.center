using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Center.Persistence.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class RootRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "root",
                schema: "id",
                table: "roles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "root",
                schema: "id",
                table: "roles");
        }
    }
}
