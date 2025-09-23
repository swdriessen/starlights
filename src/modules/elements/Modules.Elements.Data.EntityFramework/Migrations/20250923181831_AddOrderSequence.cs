using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Elements.Data.EntityFramework.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddOrderSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_element_component_statistic_rule_owning_element_id",
                schema: "elements",
                table: "element_component_statistic_rule");

            migrationBuilder.DropIndex(
                name: "IX_element_component_sorting_owning_element_id",
                schema: "elements",
                table: "element_component_sorting");

            migrationBuilder.DropIndex(
                name: "IX_element_component_short_description_owning_element_id",
                schema: "elements",
                table: "element_component_short_description");

            migrationBuilder.DropIndex(
                name: "IX_element_component_selection_rule_owning_element_id",
                schema: "elements",
                table: "element_component_selection_rule");

            migrationBuilder.DropIndex(
                name: "IX_element_component_primary_ability_owning_element_id",
                schema: "elements",
                table: "element_component_primary_ability");

            migrationBuilder.DropIndex(
                name: "IX_element_component_prerequisites_owning_element_id",
                schema: "elements",
                table: "element_component_prerequisites");

            migrationBuilder.DropIndex(
                name: "IX_element_component_language_owning_element_id",
                schema: "elements",
                table: "element_component_language");

            migrationBuilder.DropIndex(
                name: "IX_element_component_include_rule_owning_element_id",
                schema: "elements",
                table: "element_component_include_rule");

            migrationBuilder.DropIndex(
                name: "IX_element_component_description_owning_element_id",
                schema: "elements",
                table: "element_component_description");

            migrationBuilder.DropIndex(
                name: "IX_element_component_ability_owning_element_id",
                schema: "elements",
                table: "element_component_ability");

            migrationBuilder.DropIndex(
                name: "IX_element_component_abbreviation_owning_element_id",
                schema: "elements",
                table: "element_component_abbreviation");

            migrationBuilder.AddColumn<int>(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_statistic_rule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_sorting",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_short_description",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_selection_rule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_primary_ability",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_prerequisites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_language",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_include_rule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_description",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_ability",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_abbreviation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_statistic_rule_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_statistic_rule",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_sorting_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_sorting",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_short_description_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_short_description",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_selection_rule_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_selection_rule",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_primary_ability_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_primary_ability",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_prerequisites_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_prerequisites",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_language_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_language",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_include_rule_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_include_rule",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_description_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_description",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_ability_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_ability",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_abbreviation_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_abbreviation",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_element_component_statistic_rule_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_statistic_rule");

            migrationBuilder.DropIndex(
                name: "IX_element_component_sorting_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_sorting");

            migrationBuilder.DropIndex(
                name: "IX_element_component_short_description_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_short_description");

            migrationBuilder.DropIndex(
                name: "IX_element_component_selection_rule_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_selection_rule");

            migrationBuilder.DropIndex(
                name: "IX_element_component_primary_ability_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_primary_ability");

            migrationBuilder.DropIndex(
                name: "IX_element_component_prerequisites_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_prerequisites");

            migrationBuilder.DropIndex(
                name: "IX_element_component_language_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_language");

            migrationBuilder.DropIndex(
                name: "IX_element_component_include_rule_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_include_rule");

            migrationBuilder.DropIndex(
                name: "IX_element_component_description_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_description");

            migrationBuilder.DropIndex(
                name: "IX_element_component_ability_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_ability");

            migrationBuilder.DropIndex(
                name: "IX_element_component_abbreviation_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_abbreviation");

            migrationBuilder.DropColumn(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_statistic_rule");

            migrationBuilder.DropColumn(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_sorting");

            migrationBuilder.DropColumn(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_short_description");

            migrationBuilder.DropColumn(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_selection_rule");

            migrationBuilder.DropColumn(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_primary_ability");

            migrationBuilder.DropColumn(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_prerequisites");

            migrationBuilder.DropColumn(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_language");

            migrationBuilder.DropColumn(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_include_rule");

            migrationBuilder.DropColumn(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_description");

            migrationBuilder.DropColumn(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_ability");

            migrationBuilder.DropColumn(
                name: "component_order_sequence",
                schema: "elements",
                table: "element_component_abbreviation");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_statistic_rule_owning_element_id",
                schema: "elements",
                table: "element_component_statistic_rule",
                column: "owning_element_id");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_sorting_owning_element_id",
                schema: "elements",
                table: "element_component_sorting",
                column: "owning_element_id");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_short_description_owning_element_id",
                schema: "elements",
                table: "element_component_short_description",
                column: "owning_element_id");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_selection_rule_owning_element_id",
                schema: "elements",
                table: "element_component_selection_rule",
                column: "owning_element_id");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_primary_ability_owning_element_id",
                schema: "elements",
                table: "element_component_primary_ability",
                column: "owning_element_id");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_prerequisites_owning_element_id",
                schema: "elements",
                table: "element_component_prerequisites",
                column: "owning_element_id");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_language_owning_element_id",
                schema: "elements",
                table: "element_component_language",
                column: "owning_element_id");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_include_rule_owning_element_id",
                schema: "elements",
                table: "element_component_include_rule",
                column: "owning_element_id");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_description_owning_element_id",
                schema: "elements",
                table: "element_component_description",
                column: "owning_element_id");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_ability_owning_element_id",
                schema: "elements",
                table: "element_component_ability",
                column: "owning_element_id");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_abbreviation_owning_element_id",
                schema: "elements",
                table: "element_component_abbreviation",
                column: "owning_element_id");
        }
    }
}
