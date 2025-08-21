using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Elements.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SnakeCaseColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_element_component_abbreviation_element_OwningElement",
                schema: "elements",
                table: "element_component_abbreviation");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_ability_element_OwningElement",
                schema: "elements",
                table: "element_component_ability");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_description_element_OwningElement",
                schema: "elements",
                table: "element_component_description");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_include_rule_element_OwningElement",
                schema: "elements",
                table: "element_component_include_rule");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_language_element_OwningElement",
                schema: "elements",
                table: "element_component_language");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_prerequisites_element_OwningElement",
                schema: "elements",
                table: "element_component_prerequisites");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_primary_ability_element_OwningElement",
                schema: "elements",
                table: "element_component_primary_ability");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_selection_rule_element_OwningElement",
                schema: "elements",
                table: "element_component_selection_rule");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_short_description_element_OwningElement",
                schema: "elements",
                table: "element_component_short_description");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_sorting_element_OwningElement",
                schema: "elements",
                table: "element_component_sorting");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_statistic_rule_element_OwningElement",
                schema: "elements",
                table: "element_component_statistic_rule");

            migrationBuilder.RenameColumn(
                name: "Value",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Requirements",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "requirements");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "StackingBonus",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "stacking_bonus");

            migrationBuilder.RenameColumn(
                name: "OwningElement",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "owning_element_id");

            migrationBuilder.RenameColumn(
                name: "LevelRequirement",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "level_requirement");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "display_name");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_statistic_rule_OwningElement",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "IX_element_component_statistic_rule_owning_element_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "elements",
                table: "element_component_sorting",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SortingOrder",
                schema: "elements",
                table: "element_component_sorting",
                newName: "sorting_order");

            migrationBuilder.RenameColumn(
                name: "OwningElement",
                schema: "elements",
                table: "element_component_sorting",
                newName: "owning_element_id");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_sorting_OwningElement",
                schema: "elements",
                table: "element_component_sorting",
                newName: "IX_element_component_sorting_owning_element_id");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "elements",
                table: "element_component_short_description",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "elements",
                table: "element_component_short_description",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "OwningElement",
                schema: "elements",
                table: "element_component_short_description",
                newName: "owning_element_id");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_short_description_OwningElement",
                schema: "elements",
                table: "element_component_short_description",
                newName: "IX_element_component_short_description_owning_element_id");

            migrationBuilder.RenameColumn(
                name: "Supports",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "supports");

            migrationBuilder.RenameColumn(
                name: "Requirements",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "requirements");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ShortDescription",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "short_description");

            migrationBuilder.RenameColumn(
                name: "RangeSupports",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "range_supports");

            migrationBuilder.RenameColumn(
                name: "OwningElement",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "owning_element_id");

            migrationBuilder.RenameColumn(
                name: "LevelRequirement",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "level_requirement");

            migrationBuilder.RenameColumn(
                name: "IsOptional",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "is_optional");

            migrationBuilder.RenameColumn(
                name: "ElementType",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "element_type");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_selection_rule_OwningElement",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "IX_element_component_selection_rule_owning_element_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "elements",
                table: "element_component_primary_ability",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PrimaryAbility",
                schema: "elements",
                table: "element_component_primary_ability",
                newName: "primary_ability_id");

            migrationBuilder.RenameColumn(
                name: "OwningElement",
                schema: "elements",
                table: "element_component_primary_ability",
                newName: "owning_element_id");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_primary_ability_OwningElement",
                schema: "elements",
                table: "element_component_primary_ability",
                newName: "IX_element_component_primary_ability_owning_element_id");

            migrationBuilder.RenameColumn(
                name: "Prerequisites",
                schema: "elements",
                table: "element_component_prerequisites",
                newName: "prerequisites");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "elements",
                table: "element_component_prerequisites",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "OwningElement",
                schema: "elements",
                table: "element_component_prerequisites",
                newName: "owning_element_id");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_prerequisites_OwningElement",
                schema: "elements",
                table: "element_component_prerequisites",
                newName: "IX_element_component_prerequisites_owning_element_id");

            migrationBuilder.RenameColumn(
                name: "Origin",
                schema: "elements",
                table: "element_component_language",
                newName: "origin");

            migrationBuilder.RenameColumn(
                name: "Kind",
                schema: "elements",
                table: "element_component_language",
                newName: "kind");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "elements",
                table: "element_component_language",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "OwningElement",
                schema: "elements",
                table: "element_component_language",
                newName: "owning_element_id");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_language_OwningElement",
                schema: "elements",
                table: "element_component_language",
                newName: "IX_element_component_language_owning_element_id");

            migrationBuilder.RenameColumn(
                name: "Requirements",
                schema: "elements",
                table: "element_component_include_rule",
                newName: "requirements");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "elements",
                table: "element_component_include_rule",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "OwningElement",
                schema: "elements",
                table: "element_component_include_rule",
                newName: "owning_element_id");

            migrationBuilder.RenameColumn(
                name: "LevelRequirement",
                schema: "elements",
                table: "element_component_include_rule",
                newName: "level_requirement");

            migrationBuilder.RenameColumn(
                name: "IncludeElement",
                schema: "elements",
                table: "element_component_include_rule",
                newName: "include_element_id");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_include_rule_OwningElement",
                schema: "elements",
                table: "element_component_include_rule",
                newName: "IX_element_component_include_rule_owning_element_id");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "elements",
                table: "element_component_description",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "elements",
                table: "element_component_description",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "OwningElement",
                schema: "elements",
                table: "element_component_description",
                newName: "owning_element_id");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_description_OwningElement",
                schema: "elements",
                table: "element_component_description",
                newName: "IX_element_component_description_owning_element_id");

            migrationBuilder.RenameColumn(
                name: "Abbreviation",
                schema: "elements",
                table: "element_component_ability",
                newName: "abbreviation");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "elements",
                table: "element_component_ability",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "OwningElement",
                schema: "elements",
                table: "element_component_ability",
                newName: "owning_element_id");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_ability_OwningElement",
                schema: "elements",
                table: "element_component_ability",
                newName: "IX_element_component_ability_owning_element_id");

            migrationBuilder.RenameColumn(
                name: "Abbreviation",
                schema: "elements",
                table: "element_component_abbreviation",
                newName: "abbreviation");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "elements",
                table: "element_component_abbreviation",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "OwningElement",
                schema: "elements",
                table: "element_component_abbreviation",
                newName: "owning_element_id");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_abbreviation_OwningElement",
                schema: "elements",
                table: "element_component_abbreviation",
                newName: "IX_element_component_abbreviation_owning_element_id");

            migrationBuilder.RenameColumn(
                name: "Type",
                schema: "elements",
                table: "element",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "elements",
                table: "element",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "elements",
                table: "element",
                newName: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_abbreviation_element_owning_element_id",
                schema: "elements",
                table: "element_component_abbreviation",
                column: "owning_element_id",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_ability_element_owning_element_id",
                schema: "elements",
                table: "element_component_ability",
                column: "owning_element_id",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_description_element_owning_element_id",
                schema: "elements",
                table: "element_component_description",
                column: "owning_element_id",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_include_rule_element_owning_element_id",
                schema: "elements",
                table: "element_component_include_rule",
                column: "owning_element_id",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_language_element_owning_element_id",
                schema: "elements",
                table: "element_component_language",
                column: "owning_element_id",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_prerequisites_element_owning_element_id",
                schema: "elements",
                table: "element_component_prerequisites",
                column: "owning_element_id",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_primary_ability_element_owning_element_id",
                schema: "elements",
                table: "element_component_primary_ability",
                column: "owning_element_id",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_selection_rule_element_owning_element_id",
                schema: "elements",
                table: "element_component_selection_rule",
                column: "owning_element_id",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_short_description_element_owning_element_id",
                schema: "elements",
                table: "element_component_short_description",
                column: "owning_element_id",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_sorting_element_owning_element_id",
                schema: "elements",
                table: "element_component_sorting",
                column: "owning_element_id",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_statistic_rule_element_owning_element_id",
                schema: "elements",
                table: "element_component_statistic_rule",
                column: "owning_element_id",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_element_component_abbreviation_element_owning_element_id",
                schema: "elements",
                table: "element_component_abbreviation");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_ability_element_owning_element_id",
                schema: "elements",
                table: "element_component_ability");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_description_element_owning_element_id",
                schema: "elements",
                table: "element_component_description");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_include_rule_element_owning_element_id",
                schema: "elements",
                table: "element_component_include_rule");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_language_element_owning_element_id",
                schema: "elements",
                table: "element_component_language");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_prerequisites_element_owning_element_id",
                schema: "elements",
                table: "element_component_prerequisites");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_primary_ability_element_owning_element_id",
                schema: "elements",
                table: "element_component_primary_ability");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_selection_rule_element_owning_element_id",
                schema: "elements",
                table: "element_component_selection_rule");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_short_description_element_owning_element_id",
                schema: "elements",
                table: "element_component_short_description");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_sorting_element_owning_element_id",
                schema: "elements",
                table: "element_component_sorting");

            migrationBuilder.DropForeignKey(
                name: "FK_element_component_statistic_rule_element_owning_element_id",
                schema: "elements",
                table: "element_component_statistic_rule");

            migrationBuilder.RenameColumn(
                name: "value",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "requirements",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "Requirements");

            migrationBuilder.RenameColumn(
                name: "name",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "stacking_bonus",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "StackingBonus");

            migrationBuilder.RenameColumn(
                name: "owning_element_id",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "OwningElement");

            migrationBuilder.RenameColumn(
                name: "level_requirement",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "LevelRequirement");

            migrationBuilder.RenameColumn(
                name: "display_name",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "DisplayName");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_statistic_rule_owning_element_id",
                schema: "elements",
                table: "element_component_statistic_rule",
                newName: "IX_element_component_statistic_rule_OwningElement");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "elements",
                table: "element_component_sorting",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "sorting_order",
                schema: "elements",
                table: "element_component_sorting",
                newName: "SortingOrder");

            migrationBuilder.RenameColumn(
                name: "owning_element_id",
                schema: "elements",
                table: "element_component_sorting",
                newName: "OwningElement");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_sorting_owning_element_id",
                schema: "elements",
                table: "element_component_sorting",
                newName: "IX_element_component_sorting_OwningElement");

            migrationBuilder.RenameColumn(
                name: "content",
                schema: "elements",
                table: "element_component_short_description",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "elements",
                table: "element_component_short_description",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "owning_element_id",
                schema: "elements",
                table: "element_component_short_description",
                newName: "OwningElement");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_short_description_owning_element_id",
                schema: "elements",
                table: "element_component_short_description",
                newName: "IX_element_component_short_description_OwningElement");

            migrationBuilder.RenameColumn(
                name: "supports",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "Supports");

            migrationBuilder.RenameColumn(
                name: "requirements",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "Requirements");

            migrationBuilder.RenameColumn(
                name: "quantity",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "name",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "short_description",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "ShortDescription");

            migrationBuilder.RenameColumn(
                name: "range_supports",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "RangeSupports");

            migrationBuilder.RenameColumn(
                name: "owning_element_id",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "OwningElement");

            migrationBuilder.RenameColumn(
                name: "level_requirement",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "LevelRequirement");

            migrationBuilder.RenameColumn(
                name: "is_optional",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "IsOptional");

            migrationBuilder.RenameColumn(
                name: "element_type",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "ElementType");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_selection_rule_owning_element_id",
                schema: "elements",
                table: "element_component_selection_rule",
                newName: "IX_element_component_selection_rule_OwningElement");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "elements",
                table: "element_component_primary_ability",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "primary_ability_id",
                schema: "elements",
                table: "element_component_primary_ability",
                newName: "PrimaryAbility");

            migrationBuilder.RenameColumn(
                name: "owning_element_id",
                schema: "elements",
                table: "element_component_primary_ability",
                newName: "OwningElement");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_primary_ability_owning_element_id",
                schema: "elements",
                table: "element_component_primary_ability",
                newName: "IX_element_component_primary_ability_OwningElement");

            migrationBuilder.RenameColumn(
                name: "prerequisites",
                schema: "elements",
                table: "element_component_prerequisites",
                newName: "Prerequisites");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "elements",
                table: "element_component_prerequisites",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "owning_element_id",
                schema: "elements",
                table: "element_component_prerequisites",
                newName: "OwningElement");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_prerequisites_owning_element_id",
                schema: "elements",
                table: "element_component_prerequisites",
                newName: "IX_element_component_prerequisites_OwningElement");

            migrationBuilder.RenameColumn(
                name: "origin",
                schema: "elements",
                table: "element_component_language",
                newName: "Origin");

            migrationBuilder.RenameColumn(
                name: "kind",
                schema: "elements",
                table: "element_component_language",
                newName: "Kind");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "elements",
                table: "element_component_language",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "owning_element_id",
                schema: "elements",
                table: "element_component_language",
                newName: "OwningElement");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_language_owning_element_id",
                schema: "elements",
                table: "element_component_language",
                newName: "IX_element_component_language_OwningElement");

            migrationBuilder.RenameColumn(
                name: "requirements",
                schema: "elements",
                table: "element_component_include_rule",
                newName: "Requirements");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "elements",
                table: "element_component_include_rule",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "owning_element_id",
                schema: "elements",
                table: "element_component_include_rule",
                newName: "OwningElement");

            migrationBuilder.RenameColumn(
                name: "level_requirement",
                schema: "elements",
                table: "element_component_include_rule",
                newName: "LevelRequirement");

            migrationBuilder.RenameColumn(
                name: "include_element_id",
                schema: "elements",
                table: "element_component_include_rule",
                newName: "IncludeElement");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_include_rule_owning_element_id",
                schema: "elements",
                table: "element_component_include_rule",
                newName: "IX_element_component_include_rule_OwningElement");

            migrationBuilder.RenameColumn(
                name: "content",
                schema: "elements",
                table: "element_component_description",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "elements",
                table: "element_component_description",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "owning_element_id",
                schema: "elements",
                table: "element_component_description",
                newName: "OwningElement");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_description_owning_element_id",
                schema: "elements",
                table: "element_component_description",
                newName: "IX_element_component_description_OwningElement");

            migrationBuilder.RenameColumn(
                name: "abbreviation",
                schema: "elements",
                table: "element_component_ability",
                newName: "Abbreviation");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "elements",
                table: "element_component_ability",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "owning_element_id",
                schema: "elements",
                table: "element_component_ability",
                newName: "OwningElement");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_ability_owning_element_id",
                schema: "elements",
                table: "element_component_ability",
                newName: "IX_element_component_ability_OwningElement");

            migrationBuilder.RenameColumn(
                name: "abbreviation",
                schema: "elements",
                table: "element_component_abbreviation",
                newName: "Abbreviation");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "elements",
                table: "element_component_abbreviation",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "owning_element_id",
                schema: "elements",
                table: "element_component_abbreviation",
                newName: "OwningElement");

            migrationBuilder.RenameIndex(
                name: "IX_element_component_abbreviation_owning_element_id",
                schema: "elements",
                table: "element_component_abbreviation",
                newName: "IX_element_component_abbreviation_OwningElement");

            migrationBuilder.RenameColumn(
                name: "type",
                schema: "elements",
                table: "element",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "name",
                schema: "elements",
                table: "element",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "elements",
                table: "element",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_abbreviation_element_OwningElement",
                schema: "elements",
                table: "element_component_abbreviation",
                column: "OwningElement",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_ability_element_OwningElement",
                schema: "elements",
                table: "element_component_ability",
                column: "OwningElement",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_description_element_OwningElement",
                schema: "elements",
                table: "element_component_description",
                column: "OwningElement",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_include_rule_element_OwningElement",
                schema: "elements",
                table: "element_component_include_rule",
                column: "OwningElement",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_language_element_OwningElement",
                schema: "elements",
                table: "element_component_language",
                column: "OwningElement",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_prerequisites_element_OwningElement",
                schema: "elements",
                table: "element_component_prerequisites",
                column: "OwningElement",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_primary_ability_element_OwningElement",
                schema: "elements",
                table: "element_component_primary_ability",
                column: "OwningElement",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_selection_rule_element_OwningElement",
                schema: "elements",
                table: "element_component_selection_rule",
                column: "OwningElement",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_short_description_element_OwningElement",
                schema: "elements",
                table: "element_component_short_description",
                column: "OwningElement",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_sorting_element_OwningElement",
                schema: "elements",
                table: "element_component_sorting",
                column: "OwningElement",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_element_component_statistic_rule_element_OwningElement",
                schema: "elements",
                table: "element_component_statistic_rule",
                column: "OwningElement",
                principalSchema: "elements",
                principalTable: "element",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
