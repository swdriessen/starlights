using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations;

/// <inheritdoc />
public partial class _20261125150237_CharacterInitialization : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ruleset_id",
            schema: "characters",
            table: "character",
            type: "nvarchar(32)",
            maxLength: 32,
            nullable: true);

        migrationBuilder.CreateTable(
            name: "character_build",
            schema: "characters",
            columns: table => new
            {
                character_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                pack_id = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                pack_version = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                character_element_id = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                composition = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_character_build", x => x.character_id);
                table.ForeignKey(
                    name: "FK_character_build_character_character_id",
                    column: x => x.character_id,
                    principalSchema: "characters",
                    principalTable: "character",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "character_build",
            schema: "characters");

        migrationBuilder.DropColumn(
            name: "ruleset_id",
            schema: "characters",
            table: "character");
    }
}
