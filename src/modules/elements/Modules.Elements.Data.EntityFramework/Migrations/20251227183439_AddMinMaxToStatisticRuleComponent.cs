using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Elements.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddMinMaxToStatisticRuleComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "maximum",
                schema: "elements",
                table: "element_component_statistic_rule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "minimum",
                schema: "elements",
                table: "element_component_statistic_rule",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "maximum",
                schema: "elements",
                table: "element_component_statistic_rule");

            migrationBuilder.DropColumn(
                name: "minimum",
                schema: "elements",
                table: "element_component_statistic_rule");
        }
    }
}
