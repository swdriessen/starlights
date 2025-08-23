using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_character_progression_parent_character",
                schema: "characters",
                table: "character_progression",
                column: "parent_character");

            migrationBuilder.AddForeignKey(
                name: "FK_character_progression_character_parent_character",
                schema: "characters",
                table: "character_progression",
                column: "parent_character",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_character_progression_character_parent_character",
                schema: "characters",
                table: "character_progression");

            migrationBuilder.DropIndex(
                name: "IX_character_progression_parent_character",
                schema: "characters",
                table: "character_progression");
        }
    }
}
