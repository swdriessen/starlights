using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddRegistrationStatisticsRuleChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LevelRequirement",
                schema: "characters",
                table: "registration_statistic_rules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StackingBonus",
                schema: "characters",
                table: "registration_statistic_rules",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LevelRequirement",
                schema: "characters",
                table: "registration_statistic_rules");

            migrationBuilder.DropColumn(
                name: "StackingBonus",
                schema: "characters",
                table: "registration_statistic_rules");
        }
    }
}
