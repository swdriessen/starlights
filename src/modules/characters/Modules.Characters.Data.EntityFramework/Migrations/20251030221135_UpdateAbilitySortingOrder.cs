using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAbilitySortingOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "sorting_order",
                schema: "characters",
                table: "skills",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "sorting_order",
                schema: "characters",
                table: "savingthrows",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "sorting_order",
                schema: "characters",
                table: "ability_scores",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sorting_order",
                schema: "characters",
                table: "skills");

            migrationBuilder.DropColumn(
                name: "sorting_order",
                schema: "characters",
                table: "savingthrows");

            migrationBuilder.DropColumn(
                name: "sorting_order",
                schema: "characters",
                table: "ability_scores");
        }
    }
}
