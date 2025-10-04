using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ProficiencyComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "component_proficiency",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    parent_character = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    proficiency_bonus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_component_proficiency", x => x.id);
                    table.ForeignKey(
                        name: "FK_component_proficiency_character_parent_character",
                        column: x => x.parent_character,
                        principalSchema: "characters",
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_component_proficiency_parent_character",
                schema: "characters",
                table: "component_proficiency",
                column: "parent_character");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "component_proficiency",
                schema: "characters");
        }
    }
}
