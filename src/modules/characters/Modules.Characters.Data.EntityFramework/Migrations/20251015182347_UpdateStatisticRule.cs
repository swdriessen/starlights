using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStatisticRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StackingBonus",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "stacking_bonus");

            migrationBuilder.RenameColumn(
                name: "LevelRequirement",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "level_requirement");

            migrationBuilder.AlterColumn<string>(
                name: "stacking_bonus",
                schema: "characters",
                table: "registration_statistic_rules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "max_value",
                schema: "characters",
                table: "registration_statistic_rules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "min_value",
                schema: "characters",
                table: "registration_statistic_rules",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "max_value",
                schema: "characters",
                table: "registration_statistic_rules");

            migrationBuilder.DropColumn(
                name: "min_value",
                schema: "characters",
                table: "registration_statistic_rules");

            migrationBuilder.RenameColumn(
                name: "stacking_bonus",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "StackingBonus");

            migrationBuilder.RenameColumn(
                name: "level_requirement",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "LevelRequirement");

            migrationBuilder.AlterColumn<string>(
                name: "StackingBonus",
                schema: "characters",
                table: "registration_statistic_rules",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
