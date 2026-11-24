using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations;

/// <inheritdoc />
public partial class _20261124221125_CharacterOwnership : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "owner_id",
            schema: "characters",
            table: "character",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateIndex(
            name: "IX_character_owner_id",
            schema: "characters",
            table: "character",
            column: "owner_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_character_owner_id",
            schema: "characters",
            table: "character");

        migrationBuilder.DropColumn(
            name: "owner_id",
            schema: "characters",
            table: "character");
    }
}
