using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddCalculatedFieldsToAbilityScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CalculatedModifier",
                schema: "characters",
                table: "ability_scores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CalculatedScore",
                schema: "characters",
                table: "ability_scores",
                type: "int",
                nullable: false,
                defaultValue: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculatedModifier",
                schema: "characters",
                table: "ability_scores");

            migrationBuilder.DropColumn(
                name: "CalculatedScore",
                schema: "characters",
                table: "ability_scores");
        }
    }
}
