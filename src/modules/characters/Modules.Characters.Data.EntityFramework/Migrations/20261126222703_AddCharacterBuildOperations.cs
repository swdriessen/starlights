using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations;

/// <inheritdoc />
public partial class _20261126222703_AddCharacterBuildOperations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "failure",
            schema: "characters",
            table: "character_build",
            type: "nvarchar(200)",
            maxLength: 200,
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "operation_id",
            schema: "characters",
            table: "character_build",
            type: "uniqueidentifier",
            nullable: true);

        migrationBuilder.AddColumn<long>(
            name: "revision",
            schema: "characters",
            table: "character_build",
            type: "bigint",
            nullable: false,
            defaultValue: 0L);

        migrationBuilder.AddColumn<string>(
            name: "sheet",
            schema: "characters",
            table: "character_build",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<long>(
            name: "sheet_revision",
            schema: "characters",
            table: "character_build",
            type: "bigint",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "status",
            schema: "characters",
            table: "character_build",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "ready");

        migrationBuilder.CreateTable(
            name: "character_edit_operations",
            schema: "characters",
            columns: table => new
            {
                character_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                operation_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                revision = table.Column<long>(type: "bigint", nullable: false),
                fingerprint = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                failure = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                sheet = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_character_edit_operations", x => new { x.character_id, x.operation_id });
                table.ForeignKey(
                    name: "FK_character_edit_operations_character_build_character_id",
                    column: x => x.character_id,
                    principalSchema: "characters",
                    principalTable: "character_build",
                    principalColumn: "character_id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_character_edit_operations_character_id_revision",
            schema: "characters",
            table: "character_edit_operations",
            columns: new[] { "character_id", "revision" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_character_edit_operations_status",
            schema: "characters",
            table: "character_edit_operations",
            column: "status");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "character_edit_operations",
            schema: "characters");

        migrationBuilder.DropColumn(
            name: "failure",
            schema: "characters",
            table: "character_build");

        migrationBuilder.DropColumn(
            name: "operation_id",
            schema: "characters",
            table: "character_build");

        migrationBuilder.DropColumn(
            name: "revision",
            schema: "characters",
            table: "character_build");

        migrationBuilder.DropColumn(
            name: "sheet",
            schema: "characters",
            table: "character_build");

        migrationBuilder.DropColumn(
            name: "sheet_revision",
            schema: "characters",
            table: "character_build");

        migrationBuilder.DropColumn(
            name: "status",
            schema: "characters",
            table: "character_build");
    }
}
