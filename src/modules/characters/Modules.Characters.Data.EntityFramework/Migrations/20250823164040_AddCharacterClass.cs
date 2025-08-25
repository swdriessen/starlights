using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_component_class",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    parent_character = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_component_class", x => x.id);
                    table.ForeignKey(
                        name: "FK_character_component_class_character_parent_character",
                        column: x => x.parent_character,
                        principalSchema: "characters",
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_class",
                schema: "characters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Registration = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ClassComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_class", x => x.Id);
                    table.ForeignKey(
                        name: "FK_character_class_character_component_class_ClassComponentId",
                        column: x => x.ClassComponentId,
                        principalSchema: "characters",
                        principalTable: "character_component_class",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_character_class_ClassComponentId",
                schema: "characters",
                table: "character_class",
                column: "ClassComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_character_class_Registration",
                schema: "characters",
                table: "character_class",
                column: "Registration");

            migrationBuilder.CreateIndex(
                name: "IX_character_component_class_parent_character",
                schema: "characters",
                table: "character_component_class",
                column: "parent_character");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_class",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "character_component_class",
                schema: "characters");
        }
    }
}
