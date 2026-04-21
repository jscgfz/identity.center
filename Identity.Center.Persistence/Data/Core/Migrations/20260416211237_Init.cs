using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Identity.Center.Persistence.Data.Core.Migrations
{
  /// <inheritdoc />
  public partial class Init : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.EnsureSchema(
          name: "sec");

      migrationBuilder.EnsureSchema(
          name: "auth");

      migrationBuilder.EnsureSchema(
          name: "build");

      migrationBuilder.EnsureSchema(
          name: "id");

      migrationBuilder.CreateTable(
          name: "actions",
          schema: "sec",
          columns: table => new
          {
            id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
            created_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "getutcdate()"),
            created_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
            last_modified_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            last_modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            deleted_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            deleted_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            name = table.Column<string>(type: "nvarchar(450)", nullable: false),
            description = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_actions", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "apps",
          schema: "build",
          columns: table => new
          {
            id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
            index = table.Column<long>(type: "bigint", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            prefix = table.Column<string>(type: "nvarchar(450)", nullable: false),
            domain_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            created_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "getutcdate()"),
            created_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
            last_modified_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            last_modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            deleted_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            deleted_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            name = table.Column<string>(type: "nvarchar(450)", nullable: false),
            description = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_apps", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "contact_types",
          schema: "sec",
          columns: table => new
          {
            id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
            key = table.Column<string>(type: "nvarchar(450)", nullable: false),
            created_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "getutcdate()"),
            created_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
            last_modified_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            last_modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            deleted_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            deleted_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            name = table.Column<string>(type: "nvarchar(450)", nullable: false),
            description = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_contact_types", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "credential_types",
          schema: "build",
          columns: table => new
          {
            id = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            auth_method_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
            arguments = table.Column<byte[]>(type: "varbinary(max)", nullable: false, defaultValue: new byte[] { 123, 125 }),
            created_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "getutcdate()"),
            created_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
            last_modified_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            last_modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            deleted_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            deleted_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            name = table.Column<string>(type: "nvarchar(450)", nullable: false),
            description = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_credential_types", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "groups",
          schema: "sec",
          columns: table => new
          {
            id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
            created_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "getutcdate()"),
            created_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
            last_modified_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            last_modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            deleted_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            deleted_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            name = table.Column<string>(type: "nvarchar(450)", nullable: false),
            description = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_groups", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "users",
          schema: "id",
          columns: table => new
          {
            id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
            document_type = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
            document_number = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
            first_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
            second_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
            first_lastname = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
            second_lastname = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
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
            table.PrimaryKey("PK_users", x => x.id);
          });

      migrationBuilder.CreateTable(
          name: "api_keys",
          schema: "auth",
          columns: table => new
          {
            id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
            app_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            hash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
            salt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
            root = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            created_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "getutcdate()"),
            created_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
            last_modified_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            last_modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            deleted_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            deleted_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            name = table.Column<string>(type: "nvarchar(450)", nullable: false),
            description = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_api_keys", x => x.id);
            table.ForeignKey(
                      name: "FK_api_keys_apps_app_id",
                      column: x => x.app_id,
                      principalSchema: "build",
                      principalTable: "apps",
                      principalColumn: "id");
          });

      migrationBuilder.CreateTable(
          name: "app_auth",
          schema: "sec",
          columns: table => new
          {
            app_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            signature_key = table.Column<byte[]>(type: "varbinary(900)", nullable: false),
            two_factor_enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            expiration_time = table.Column<TimeSpan>(type: "time", nullable: false, defaultValue: new TimeSpan(0, 1, 0, 0, 0)),
            refresh_time = table.Column<TimeSpan>(type: "time", nullable: false, defaultValue: new TimeSpan(0, 1, 30, 0, 0)),
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
            table.PrimaryKey("PK_app_auth", x => x.app_id);
            table.ForeignKey(
                      name: "FK_app_auth_apps_app_id",
                      column: x => x.app_id,
                      principalSchema: "build",
                      principalTable: "apps",
                      principalColumn: "id");
          });

      migrationBuilder.CreateTable(
          name: "app_settings",
          schema: "build",
          columns: table => new
          {
            app_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            key = table.Column<string>(type: "nvarchar(450)", nullable: false),
            value = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
            table.PrimaryKey("PK_app_settings", x => new { x.app_id, x.key });
            table.ForeignKey(
                      name: "FK_app_settings_apps_app_id",
                      column: x => x.app_id,
                      principalSchema: "build",
                      principalTable: "apps",
                      principalColumn: "id");
          });

      migrationBuilder.CreateTable(
          name: "healtchecks",
          schema: "build",
          columns: table => new
          {
            id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
            app_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            healtcheck_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
            arguments = table.Column<byte[]>(type: "varbinary(max)", nullable: false, defaultValue: new byte[] { 123, 125 }),
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
            table.PrimaryKey("PK_healtchecks", x => x.id);
            table.ForeignKey(
                      name: "FK_healtchecks_apps_app_id",
                      column: x => x.app_id,
                      principalSchema: "build",
                      principalTable: "apps",
                      principalColumn: "id");
          });

      migrationBuilder.CreateTable(
          name: "roles",
          schema: "id",
          columns: table => new
          {
            id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
            app_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            name = table.Column<string>(type: "nvarchar(450)", nullable: false),
            description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            domain_name = table.Column<string>(type: "nvarchar(450)", nullable: true),
            ad_mandatory = table.Column<bool>(type: "bit", nullable: false),
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
            table.PrimaryKey("PK_roles", x => x.id);
            table.ForeignKey(
                      name: "FK_roles_apps_app_id",
                      column: x => x.app_id,
                      principalSchema: "build",
                      principalTable: "apps",
                      principalColumn: "id");
          });

      migrationBuilder.CreateTable(
          name: "apps_allowed_credentials",
          schema: "build",
          columns: table => new
          {
            app_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            credential_type_id = table.Column<int>(type: "int", nullable: false),
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
            table.PrimaryKey("PK_apps_allowed_credentials", x => new { x.app_id, x.credential_type_id });
            table.ForeignKey(
                      name: "FK_apps_allowed_credentials_apps_app_id",
                      column: x => x.app_id,
                      principalSchema: "build",
                      principalTable: "apps",
                      principalColumn: "id");
            table.ForeignKey(
                      name: "FK_apps_allowed_credentials_credential_types_credential_type_id",
                      column: x => x.credential_type_id,
                      principalSchema: "build",
                      principalTable: "credential_types",
                      principalColumn: "id");
          });

      migrationBuilder.CreateTable(
          name: "claims",
          schema: "auth",
          columns: table => new
          {
            id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
            action_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            group_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
            table.PrimaryKey("PK_claims", x => x.id);
            table.ForeignKey(
                      name: "FK_claims_actions_action_id",
                      column: x => x.action_id,
                      principalSchema: "sec",
                      principalTable: "actions",
                      principalColumn: "id");
            table.ForeignKey(
                      name: "FK_claims_groups_group_id",
                      column: x => x.group_id,
                      principalSchema: "sec",
                      principalTable: "groups",
                      principalColumn: "id");
          });

      migrationBuilder.CreateTable(
          name: "contact_info",
          schema: "id",
          columns: table => new
          {
            id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
            user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            contact_type_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            value = table.Column<string>(type: "nvarchar(450)", nullable: false),
            salt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
            confirmed = table.Column<bool>(type: "bit", nullable: false),
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
            table.PrimaryKey("PK_contact_info", x => x.id);
            table.ForeignKey(
                      name: "FK_contact_info_contact_types_contact_type_id",
                      column: x => x.contact_type_id,
                      principalSchema: "sec",
                      principalTable: "contact_types",
                      principalColumn: "id");
            table.ForeignKey(
                      name: "FK_contact_info_users_user_id",
                      column: x => x.user_id,
                      principalSchema: "id",
                      principalTable: "users",
                      principalColumn: "id");
          });

      migrationBuilder.CreateTable(
          name: "domain_credentials",
          schema: "auth",
          columns: table => new
          {
            id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
            user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            credential_type_id = table.Column<int>(type: "int", nullable: false),
            Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
            table.PrimaryKey("PK_domain_credentials", x => x.id);
            table.ForeignKey(
                      name: "FK_domain_credentials_credential_types_credential_type_id",
                      column: x => x.credential_type_id,
                      principalSchema: "build",
                      principalTable: "credential_types",
                      principalColumn: "id");
            table.ForeignKey(
                      name: "FK_domain_credentials_users_user_id",
                      column: x => x.user_id,
                      principalSchema: "id",
                      principalTable: "users",
                      principalColumn: "id");
          });

      migrationBuilder.CreateTable(
          name: "single_credentials",
          schema: "auth",
          columns: table => new
          {
            user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            app_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            username = table.Column<string>(type: "nvarchar(450)", nullable: false),
            hash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
            salt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
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
            table.PrimaryKey("PK_single_credentials", x => new { x.app_id, x.user_id });
            table.ForeignKey(
                      name: "FK_single_credentials_apps_app_id",
                      column: x => x.app_id,
                      principalSchema: "build",
                      principalTable: "apps",
                      principalColumn: "id");
            table.ForeignKey(
                      name: "FK_single_credentials_users_user_id",
                      column: x => x.user_id,
                      principalSchema: "id",
                      principalTable: "users",
                      principalColumn: "id");
          });

      migrationBuilder.CreateTable(
          name: "users_roles",
          schema: "auth",
          columns: table => new
          {
            user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            role_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
            table.PrimaryKey("PK_users_roles", x => new { x.user_id, x.role_id });
            table.ForeignKey(
                      name: "FK_users_roles_roles_role_id",
                      column: x => x.role_id,
                      principalSchema: "id",
                      principalTable: "roles",
                      principalColumn: "id");
            table.ForeignKey(
                      name: "FK_users_roles_users_user_id",
                      column: x => x.user_id,
                      principalSchema: "id",
                      principalTable: "users",
                      principalColumn: "id");
          });

      migrationBuilder.CreateTable(
          name: "api_keys_claims",
          schema: "auth",
          columns: table => new
          {
            api_key_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            claim_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
            table.PrimaryKey("PK_api_keys_claims", x => new { x.api_key_id, x.claim_id });
            table.ForeignKey(
                      name: "FK_api_keys_claims_api_keys_api_key_id",
                      column: x => x.api_key_id,
                      principalSchema: "auth",
                      principalTable: "api_keys",
                      principalColumn: "id");
            table.ForeignKey(
                      name: "FK_api_keys_claims_claims_claim_id",
                      column: x => x.claim_id,
                      principalSchema: "auth",
                      principalTable: "claims",
                      principalColumn: "id");
          });

      migrationBuilder.CreateTable(
          name: "roles_claims",
          schema: "auth",
          columns: table => new
          {
            role_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            claim_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
            table.PrimaryKey("PK_roles_claims", x => new { x.role_id, x.claim_id });
            table.ForeignKey(
                      name: "FK_roles_claims_claims_claim_id",
                      column: x => x.claim_id,
                      principalSchema: "auth",
                      principalTable: "claims",
                      principalColumn: "id");
            table.ForeignKey(
                      name: "FK_roles_claims_roles_role_id",
                      column: x => x.role_id,
                      principalSchema: "id",
                      principalTable: "roles",
                      principalColumn: "id");
          });

      migrationBuilder.InsertData(
          schema: "build",
          table: "apps",
          columns: new[] { "id", "deleted_at_utc", "deleted_by", "description", "domain_name", "index", "last_modified_at_utc", "last_modified_by", "name", "prefix" },
          values: new object[,]
          {
                    { new Guid("085e4fa8-72a0-f011-81de-00505682eca9"), null, null, "Servicios de atención telefónica para Promotec", null, 4L, null, null, "Atenea Promotec", "apt" },
                    { new Guid("2cab2232-72a0-f011-81de-00505682eca9"), null, null, "Servicios de atención telefónica centralizados", null, 2L, null, null, "Atenea Iris", "ais" },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), null, null, "Servicio de gestión de identidad", null, 1L, null, null, "Identity", "sid" },
                    { new Guid("7b2c9ab9-77a0-f011-81de-00505682eca9"), null, null, "Servicios de atención telefónica para Carfiao", null, 7L, null, null, "Atenea Carfiao", "acf" },
                    { new Guid("99d8850b-73a0-f011-81de-00505682eca9"), null, null, "Servicios de control y monitoreo de telefonía", null, 5L, null, null, "Central Asterisk", "ast" },
                    { new Guid("befbac57-72a0-f011-81de-00505682eca9"), null, null, "Servicios de atención telefónica centralizados", null, 3L, null, null, "Atenea Asisya", "aay" },
                    { new Guid("c20bfd03-77a0-f011-81de-00505682eca9"), null, null, "Administrador de credenciales de la web de finanzauto", null, 6L, null, null, "Finanzauto Web Admin", "fzw" }
          });

      migrationBuilder.InsertData(
          schema: "sec",
          table: "contact_types",
          columns: new[] { "id", "key", "deleted_at_utc", "deleted_by", "description", "last_modified_at_utc", "last_modified_by", "name" },
          values: new object[,]
          {
                    { new Guid("433c5502-cf39-f111-81e9-00505682eca9"), "coorp-mail", null, null, "Correos internos (correos del workspace)", null, null, "Correo coorporativo" },
                    { new Guid("cc763078-cf39-f111-81e9-00505682eca9"), "external-mail", null, null, "Correos externos a la organización", null, null, "Correo externo" },
                    { new Guid("f953c4ae-cf39-f111-81e9-00505682eca9"), "cellphone", null, null, "Número de celular", null, null, "Teléfono Celular" }
          });

      migrationBuilder.InsertData(
          schema: "build",
          table: "credential_types",
          columns: new[] { "id", "arguments", "auth_method_type", "deleted_at_utc", "deleted_by", "description", "last_modified_at_utc", "last_modified_by", "name" },
          values: new object[,]
          {
                    { 1, new byte[] { 123, 125 }, "user-password", null, null, "Ingreso con usuario y contraseña generado por la aplicación", null, null, "Usuario y contraseña" },
                    { 2, new byte[] { 123, 34, 107, 101, 121, 34, 58, 34, 75, 100, 78, 69, 83, 74, 101, 73, 97, 100, 81, 92, 117, 48, 48, 50, 66, 85, 92, 117, 48, 48, 50, 66, 81, 53, 81, 115, 92, 117, 48, 48, 50, 66, 56, 66, 81, 61, 61, 34, 44, 34, 100, 111, 109, 97, 105, 110, 78, 97, 109, 101, 34, 58, 34, 70, 90, 67, 79, 82, 80, 34, 125 }, "qd-endpoint", null, null, "Ingreso con usuario de Dominio Finanzauto", null, null, "Finanzauto" },
                    { 3, new byte[] { 123, 34, 107, 101, 121, 34, 58, 34, 75, 100, 78, 69, 83, 74, 101, 73, 97, 100, 81, 92, 117, 48, 48, 50, 66, 85, 92, 117, 48, 48, 50, 66, 81, 53, 81, 115, 92, 117, 48, 48, 50, 66, 56, 66, 81, 61, 61, 34, 44, 34, 100, 111, 109, 97, 105, 110, 78, 97, 109, 101, 34, 58, 34, 81, 66, 84, 65, 34, 125 }, "qd-endpoint", null, null, "Ingreso con usuario de Dominio Quantum Data", null, null, "Quantum Data" },
                    { 4, new byte[] { 123, 34, 107, 101, 121, 34, 58, 34, 75, 100, 78, 69, 83, 74, 101, 73, 97, 100, 81, 92, 117, 48, 48, 50, 66, 85, 92, 117, 48, 48, 50, 66, 81, 53, 81, 115, 92, 117, 48, 48, 50, 66, 56, 66, 81, 61, 61, 34, 44, 34, 100, 111, 109, 97, 105, 110, 78, 97, 109, 101, 34, 58, 34, 80, 84, 83, 69, 71, 85, 82, 79, 83, 34, 125 }, "qd-endpoint", null, null, "Ingreso con usuario de Dominio Promotec", null, null, "Promotec" },
                    { 5, new byte[] { 123, 34, 107, 101, 121, 34, 58, 34, 83, 87, 74, 70, 55, 69, 92, 117, 48, 48, 50, 66, 54, 71, 114, 102, 54, 51, 67, 111, 57, 68, 106, 121, 50, 74, 119, 61, 61, 34, 44, 34, 100, 111, 109, 97, 105, 110, 78, 97, 109, 101, 34, 58, 34, 70, 90, 67, 79, 82, 80, 34, 125 }, "qd-endpoint", null, null, "Ingreso con usuario de Dominio Asisya", null, null, "Asisya" }
          });

      migrationBuilder.InsertData(
          schema: "sec",
          table: "app_auth",
          columns: new[] { "app_id", "deleted_at_utc", "deleted_by", "last_modified_at_utc", "last_modified_by", "signature_key" },
          values: new object[,]
          {
                    { new Guid("085e4fa8-72a0-f011-81de-00505682eca9"), null, null, null, null, new byte[] { 91, 226, 250, 169, 173, 40, 65, 160, 238, 110, 230, 102, 25, 36, 14, 179, 195, 15, 218, 62, 41, 138, 146, 88, 51, 114, 195, 34, 182, 196, 153, 99 } },
                    { new Guid("2cab2232-72a0-f011-81de-00505682eca9"), null, null, null, null, new byte[] { 249, 18, 34, 98, 74, 76, 224, 211, 201, 247, 70, 92, 58, 65, 182, 217, 181, 96, 181, 61, 99, 191, 241, 233, 207, 26, 209, 91, 227, 46, 15, 122 } },
                    { new Guid("5f9c0e66-e29f-f011-81de-00505682eca9"), null, null, null, null, new byte[] { 19, 161, 15, 77, 197, 84, 141, 150, 231, 18, 90, 115, 18, 1, 45, 18, 17, 246, 235, 91, 213, 252, 90, 154, 46, 179, 179, 170, 31, 83, 36, 100 } },
                    { new Guid("7b2c9ab9-77a0-f011-81de-00505682eca9"), null, null, null, null, new byte[] { 101, 206, 146, 19, 58, 149, 193, 222, 177, 24, 199, 139, 66, 42, 16, 125, 221, 209, 254, 155, 118, 208, 165, 154, 67, 252, 3, 229, 228, 15, 185, 62 } },
                    { new Guid("99d8850b-73a0-f011-81de-00505682eca9"), null, null, null, null, new byte[] { 131, 181, 168, 162, 251, 106, 215, 25, 15, 146, 239, 52, 140, 147, 47, 226, 181, 88, 117, 23, 69, 214, 234, 176, 252, 159, 247, 115, 196, 206, 54, 141 } },
                    { new Guid("befbac57-72a0-f011-81de-00505682eca9"), null, null, null, null, new byte[] { 100, 80, 12, 96, 214, 214, 199, 10, 172, 99, 43, 207, 43, 222, 15, 139, 167, 130, 34, 38, 139, 56, 253, 196, 138, 220, 102, 135, 182, 8, 128, 63 } },
                    { new Guid("c20bfd03-77a0-f011-81de-00505682eca9"), null, null, null, null, new byte[] { 225, 99, 122, 255, 85, 23, 96, 166, 23, 30, 120, 30, 179, 180, 171, 182, 255, 119, 18, 248, 132, 204, 37, 232, 186, 199, 63, 22, 46, 160, 16, 7 } }
          });

      migrationBuilder.CreateIndex(
          name: "IX_actions_name",
          schema: "sec",
          table: "actions",
          column: "name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_api_keys_app_id",
          schema: "auth",
          table: "api_keys",
          column: "app_id");

      migrationBuilder.CreateIndex(
          name: "IX_api_keys_name",
          schema: "auth",
          table: "api_keys",
          column: "name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_api_keys_claims_claim_id",
          schema: "auth",
          table: "api_keys_claims",
          column: "claim_id");

      migrationBuilder.CreateIndex(
          name: "IX_app_auth_signature_key",
          schema: "sec",
          table: "app_auth",
          column: "signature_key",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_apps_index",
          schema: "build",
          table: "apps",
          column: "index",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_apps_name",
          schema: "build",
          table: "apps",
          column: "name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_apps_prefix",
          schema: "build",
          table: "apps",
          column: "prefix",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_apps_allowed_credentials_credential_type_id",
          schema: "build",
          table: "apps_allowed_credentials",
          column: "credential_type_id");

      migrationBuilder.CreateIndex(
          name: "IX_claims_action_id",
          schema: "auth",
          table: "claims",
          column: "action_id");

      migrationBuilder.CreateIndex(
          name: "IX_claims_group_id",
          schema: "auth",
          table: "claims",
          column: "group_id");

      migrationBuilder.CreateIndex(
          name: "IX_contact_info_contact_type_id",
          schema: "id",
          table: "contact_info",
          column: "contact_type_id");

      migrationBuilder.CreateIndex(
          name: "IX_contact_info_user_id_contact_type_id_value",
          schema: "id",
          table: "contact_info",
          columns: new[] { "user_id", "contact_type_id", "value" },
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_contact_types_key",
          schema: "sec",
          table: "contact_types",
          column: "key",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_contact_types_name",
          schema: "sec",
          table: "contact_types",
          column: "name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_credential_types_name",
          schema: "build",
          table: "credential_types",
          column: "name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_domain_credentials_credential_type_id_Username",
          schema: "auth",
          table: "domain_credentials",
          columns: new[] { "credential_type_id", "Username" },
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_domain_credentials_user_id",
          schema: "auth",
          table: "domain_credentials",
          column: "user_id");

      migrationBuilder.CreateIndex(
          name: "IX_groups_name",
          schema: "sec",
          table: "groups",
          column: "name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_healtchecks_app_id",
          schema: "build",
          table: "healtchecks",
          column: "app_id");

      migrationBuilder.CreateIndex(
          name: "IX_roles_app_id",
          schema: "id",
          table: "roles",
          column: "app_id");

      migrationBuilder.CreateIndex(
          name: "IX_roles_domain_name",
          schema: "id",
          table: "roles",
          column: "domain_name",
          unique: true,
          filter: "[domain_name] IS NOT NULL");

      migrationBuilder.CreateIndex(
          name: "IX_roles_name_app_id",
          schema: "id",
          table: "roles",
          columns: new[] { "name", "app_id" },
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_roles_claims_claim_id",
          schema: "auth",
          table: "roles_claims",
          column: "claim_id");

      migrationBuilder.CreateIndex(
          name: "IX_single_credentials_app_id_username",
          schema: "auth",
          table: "single_credentials",
          columns: new[] { "app_id", "username" },
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_single_credentials_user_id",
          schema: "auth",
          table: "single_credentials",
          column: "user_id");

      migrationBuilder.CreateIndex(
          name: "IX_users_document_type_document_number",
          schema: "id",
          table: "users",
          columns: new[] { "document_type", "document_number" },
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_users_roles_role_id",
          schema: "auth",
          table: "users_roles",
          column: "role_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "api_keys_claims",
          schema: "auth");

      migrationBuilder.DropTable(
          name: "app_auth",
          schema: "sec");

      migrationBuilder.DropTable(
          name: "app_settings",
          schema: "build");

      migrationBuilder.DropTable(
          name: "apps_allowed_credentials",
          schema: "build");

      migrationBuilder.DropTable(
          name: "contact_info",
          schema: "id");

      migrationBuilder.DropTable(
          name: "domain_credentials",
          schema: "auth");

      migrationBuilder.DropTable(
          name: "healtchecks",
          schema: "build");

      migrationBuilder.DropTable(
          name: "roles_claims",
          schema: "auth");

      migrationBuilder.DropTable(
          name: "single_credentials",
          schema: "auth");

      migrationBuilder.DropTable(
          name: "users_roles",
          schema: "auth");

      migrationBuilder.DropTable(
          name: "api_keys",
          schema: "auth");

      migrationBuilder.DropTable(
          name: "contact_types",
          schema: "sec");

      migrationBuilder.DropTable(
          name: "credential_types",
          schema: "build");

      migrationBuilder.DropTable(
          name: "claims",
          schema: "auth");

      migrationBuilder.DropTable(
          name: "roles",
          schema: "id");

      migrationBuilder.DropTable(
          name: "users",
          schema: "id");

      migrationBuilder.DropTable(
          name: "actions",
          schema: "sec");

      migrationBuilder.DropTable(
          name: "groups",
          schema: "sec");

      migrationBuilder.DropTable(
          name: "apps",
          schema: "build");
    }
  }
}
