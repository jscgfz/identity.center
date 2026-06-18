using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Identity.Center.Persistence.Data.Core.Migrations
{
    /// <inheritdoc />
    public partial class AlfrescoOptions : Migration
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
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "AlfrescoOptions:BaseUrl", null, null, null, null, "http://192.168.50.76:11001/alfresco/api/-default-/public/alfresco/versions/1" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "AlfrescoOptions:NodeCollection:Authorization", null, null, null, null, "76a0b619-3b8a-44f5-a947-b3a6a4db8378" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "AlfrescoOptions:Password", null, null, null, null, "L0nd0n$.$" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "AlfrescoOptions:Username", null, null, null, null, "laura.roa" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "AlfrescoOptions:ValidMimeTypes:0", null, null, null, null, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "AlfrescoOptions:ValidMimeTypes:1", null, null, null, null, "application/pdf" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "build",
                table: "app_settings",
                keyColumns: new[] { "app_id", "key" },
                keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "AlfrescoOptions:BaseUrl" });

            migrationBuilder.DeleteData(
                schema: "build",
                table: "app_settings",
                keyColumns: new[] { "app_id", "key" },
                keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "AlfrescoOptions:NodeCollection:Authorization" });

            migrationBuilder.DeleteData(
                schema: "build",
                table: "app_settings",
                keyColumns: new[] { "app_id", "key" },
                keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "AlfrescoOptions:Password" });

            migrationBuilder.DeleteData(
                schema: "build",
                table: "app_settings",
                keyColumns: new[] { "app_id", "key" },
                keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "AlfrescoOptions:Username" });

            migrationBuilder.DeleteData(
                schema: "build",
                table: "app_settings",
                keyColumns: new[] { "app_id", "key" },
                keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "AlfrescoOptions:ValidMimeTypes:0" });

            migrationBuilder.DeleteData(
                schema: "build",
                table: "app_settings",
                keyColumns: new[] { "app_id", "key" },
                keyValues: new object[] { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), "AlfrescoOptions:ValidMimeTypes:1" });
        }
    }
}
