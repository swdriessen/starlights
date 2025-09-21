using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNamingConventions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ability_scores_character_component_abilities_component_id",
                schema: "characters",
                table: "ability_scores");

            migrationBuilder.DropForeignKey(
                name: "FK_character_class_character_component_class_component_id",
                schema: "characters",
                table: "character_class");

            migrationBuilder.DropForeignKey(
                name: "FK_character_component_abilities_character_parent_character",
                schema: "characters",
                table: "character_component_abilities");

            migrationBuilder.DropForeignKey(
                name: "FK_character_component_appearance_character_parent_character",
                schema: "characters",
                table: "character_component_appearance");

            migrationBuilder.DropForeignKey(
                name: "FK_character_component_class_character_parent_character",
                schema: "characters",
                table: "character_component_class");

            migrationBuilder.DropForeignKey(
                name: "FK_character_component_saving_throws_character_parent_character",
                schema: "characters",
                table: "character_component_saving_throws");

            migrationBuilder.DropForeignKey(
                name: "FK_character_component_skills_character_parent_character",
                schema: "characters",
                table: "character_component_skills");

            migrationBuilder.DropForeignKey(
                name: "FK_character_progression_character_parent_character",
                schema: "characters",
                table: "character_progression");

            migrationBuilder.DropForeignKey(
                name: "FK_registration_include_rules_registration_ParentRegistrationId",
                schema: "characters",
                table: "registration_include_rules");

            migrationBuilder.DropForeignKey(
                name: "FK_registration_selection_rules_registration_ParentRegistrationId",
                schema: "characters",
                table: "registration_selection_rules");

            migrationBuilder.DropForeignKey(
                name: "FK_registration_statistic_rules_registration_ParentRegistrationId",
                schema: "characters",
                table: "registration_statistic_rules");

            migrationBuilder.DropForeignKey(
                name: "FK_savingthrows_character_component_saving_throws_component_id",
                schema: "characters",
                table: "savingthrows");

            migrationBuilder.DropForeignKey(
                name: "FK_skills_character_component_skills_component_id",
                schema: "characters",
                table: "skills");

            migrationBuilder.DropIndex(
                name: "IX_skills_CharacterId",
                schema: "characters",
                table: "skills");

            migrationBuilder.DropIndex(
                name: "IX_savingthrows_CharacterId",
                schema: "characters",
                table: "savingthrows");

            migrationBuilder.DropIndex(
                name: "IX_character_class_ClassComponentId",
                schema: "characters",
                table: "character_class");

            migrationBuilder.DropIndex(
                name: "IX_ability_scores_CharacterId",
                schema: "characters",
                table: "ability_scores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_character_progression",
                schema: "characters",
                table: "character_progression");

            migrationBuilder.DropPrimaryKey(
                name: "PK_character_component_skills",
                schema: "characters",
                table: "character_component_skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_character_component_saving_throws",
                schema: "characters",
                table: "character_component_saving_throws");

            migrationBuilder.DropPrimaryKey(
                name: "PK_character_component_class",
                schema: "characters",
                table: "character_component_class");

            migrationBuilder.DropPrimaryKey(
                name: "PK_character_component_appearance",
                schema: "characters",
                table: "character_component_appearance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_character_component_abilities",
                schema: "characters",
                table: "character_component_abilities");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                schema: "characters",
                table: "skills");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                schema: "characters",
                table: "savingthrows");

            migrationBuilder.DropColumn(
                name: "ClassComponentId",
                schema: "characters",
                table: "character_class");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                schema: "characters",
                table: "ability_scores");

            migrationBuilder.RenameTable(
                name: "character_progression",
                schema: "characters",
                newName: "component_progression",
                newSchema: "characters");

            migrationBuilder.RenameTable(
                name: "character_component_skills",
                schema: "characters",
                newName: "component_skills",
                newSchema: "characters");

            migrationBuilder.RenameTable(
                name: "character_component_saving_throws",
                schema: "characters",
                newName: "component_saving_throws",
                newSchema: "characters");

            migrationBuilder.RenameTable(
                name: "character_component_class",
                schema: "characters",
                newName: "component_class",
                newSchema: "characters");

            migrationBuilder.RenameTable(
                name: "character_component_appearance",
                schema: "characters",
                newName: "component_appearance",
                newSchema: "characters");

            migrationBuilder.RenameTable(
                name: "character_component_abilities",
                schema: "characters",
                newName: "component_abilities",
                newSchema: "characters");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "characters",
                table: "skills",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "characters",
                table: "skills",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CalculatedBonus",
                schema: "characters",
                table: "skills",
                newName: "calculated_bonus");

            migrationBuilder.RenameColumn(
                name: "AssociatedRegistrationId",
                schema: "characters",
                table: "skills",
                newName: "associated_registration_id");

            migrationBuilder.RenameColumn(
                name: "AdditionalBonus",
                schema: "characters",
                table: "skills",
                newName: "additional_bonus");

            migrationBuilder.RenameColumn(
                name: "AbilityScoreModifier",
                schema: "characters",
                table: "skills",
                newName: "ability_score_modifier");

            migrationBuilder.RenameColumn(
                name: "AbilityScoreId",
                schema: "characters",
                table: "skills",
                newName: "ability_score_id");

            migrationBuilder.RenameColumn(
                name: "AbilityScoreAbbreviation",
                schema: "characters",
                table: "skills",
                newName: "ability_score_abbreviation");

            migrationBuilder.RenameColumn(
                name: "component_id",
                schema: "characters",
                table: "skills",
                newName: "parent_component_id");

            migrationBuilder.RenameIndex(
                name: "IX_skills_component_id",
                schema: "characters",
                table: "skills",
                newName: "IX_skills_parent_component_id");

            migrationBuilder.RenameIndex(
                name: "IX_skills_AssociatedRegistrationId",
                schema: "characters",
                table: "skills",
                newName: "IX_skills_associated_registration_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "characters",
                table: "savingthrows",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "characters",
                table: "savingthrows",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CalculatedBonus",
                schema: "characters",
                table: "savingthrows",
                newName: "calculated_bonus");

            migrationBuilder.RenameColumn(
                name: "AssociatedRegistrationId",
                schema: "characters",
                table: "savingthrows",
                newName: "associated_registration_id");

            migrationBuilder.RenameColumn(
                name: "AdditionalBonus",
                schema: "characters",
                table: "savingthrows",
                newName: "additional_bonus");

            migrationBuilder.RenameColumn(
                name: "AbilityScoreModifier",
                schema: "characters",
                table: "savingthrows",
                newName: "ability_score_modifier");

            migrationBuilder.RenameColumn(
                name: "AbilityScoreId",
                schema: "characters",
                table: "savingthrows",
                newName: "ability_score_id");

            migrationBuilder.RenameColumn(
                name: "AbilityScoreAbbreviation",
                schema: "characters",
                table: "savingthrows",
                newName: "ability_score_abbreviation");

            migrationBuilder.RenameColumn(
                name: "component_id",
                schema: "characters",
                table: "savingthrows",
                newName: "parent_component_id");

            migrationBuilder.RenameIndex(
                name: "IX_savingthrows_component_id",
                schema: "characters",
                table: "savingthrows",
                newName: "IX_savingthrows_parent_component_id");

            migrationBuilder.RenameIndex(
                name: "IX_savingthrows_AssociatedRegistrationId",
                schema: "characters",
                table: "savingthrows",
                newName: "IX_savingthrows_associated_registration_id");

            migrationBuilder.RenameColumn(
                name: "Value",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ParentRegistrationId",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "parent_registration_id");

            migrationBuilder.RenameColumn(
                name: "AssociatedStatisticRuleId",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "associated_statistic_rule_id");

            migrationBuilder.RenameIndex(
                name: "IX_registration_statistic_rules_ParentRegistrationId",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "IX_registration_statistic_rules_parent_registration_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ParentRegistrationId",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "parent_registration_id");

            migrationBuilder.RenameColumn(
                name: "ElementType",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "element_type");

            migrationBuilder.RenameColumn(
                name: "CurrentSelection",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "current_selection");

            migrationBuilder.RenameColumn(
                name: "AssociatedSelectionRuleId",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "associated_selection_rule_id");

            migrationBuilder.RenameIndex(
                name: "IX_registration_selection_rules_ParentRegistrationId",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "IX_registration_selection_rules_parent_registration_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "characters",
                table: "registration_include_rules",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ParentRegistrationId",
                schema: "characters",
                table: "registration_include_rules",
                newName: "parent_registration_id");

            migrationBuilder.RenameColumn(
                name: "IncludedElementName",
                schema: "characters",
                table: "registration_include_rules",
                newName: "included_element_name");

            migrationBuilder.RenameColumn(
                name: "IncludedElementId",
                schema: "characters",
                table: "registration_include_rules",
                newName: "included_element_id");

            migrationBuilder.RenameColumn(
                name: "AssociatedIncludeRuleId",
                schema: "characters",
                table: "registration_include_rules",
                newName: "associated_include_rule_id");

            migrationBuilder.RenameIndex(
                name: "IX_registration_include_rules_ParentRegistrationId",
                schema: "characters",
                table: "registration_include_rules",
                newName: "IX_registration_include_rules_parent_registration_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "characters",
                table: "registration",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ParentRegistrationId",
                schema: "characters",
                table: "registration",
                newName: "parent_registration_id");

            migrationBuilder.RenameColumn(
                name: "CharacterId",
                schema: "characters",
                table: "registration",
                newName: "character_id");

            migrationBuilder.RenameColumn(
                name: "AssociatedElementType",
                schema: "characters",
                table: "registration",
                newName: "associated_element_type");

            migrationBuilder.RenameColumn(
                name: "AssociatedElementName",
                schema: "characters",
                table: "registration",
                newName: "associated_element_name");

            migrationBuilder.RenameColumn(
                name: "AssociatedElementId",
                schema: "characters",
                table: "registration",
                newName: "associated_element_id");

            migrationBuilder.RenameIndex(
                name: "IX_registration_CharacterId",
                schema: "characters",
                table: "registration",
                newName: "IX_registration_character_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "characters",
                table: "character_class",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Level",
                schema: "characters",
                table: "character_class",
                newName: "level");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "characters",
                table: "character_class",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Registration",
                schema: "characters",
                table: "character_class",
                newName: "registration_id");

            migrationBuilder.RenameColumn(
                name: "IsPrimary",
                schema: "characters",
                table: "character_class",
                newName: "is_primary");

            migrationBuilder.RenameColumn(
                name: "component_id",
                schema: "characters",
                table: "character_class",
                newName: "parent_component_id");

            migrationBuilder.RenameIndex(
                name: "IX_character_class_Registration",
                schema: "characters",
                table: "character_class",
                newName: "IX_character_class_registration_id");

            migrationBuilder.RenameIndex(
                name: "IX_character_class_component_id",
                schema: "characters",
                table: "character_class",
                newName: "IX_character_class_parent_component_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "characters",
                table: "character",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "characters",
                table: "character",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "characters",
                table: "ability_scores",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Abbreviation",
                schema: "characters",
                table: "ability_scores",
                newName: "abbreviation");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "characters",
                table: "ability_scores",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CalculatedScore",
                schema: "characters",
                table: "ability_scores",
                newName: "calculated_score");

            migrationBuilder.RenameColumn(
                name: "CalculatedModifier",
                schema: "characters",
                table: "ability_scores",
                newName: "calculated_modifier");

            migrationBuilder.RenameColumn(
                name: "BaseScore",
                schema: "characters",
                table: "ability_scores",
                newName: "base_score");

            migrationBuilder.RenameColumn(
                name: "AssociatedRegistrationId",
                schema: "characters",
                table: "ability_scores",
                newName: "associated_registration_id");

            migrationBuilder.RenameColumn(
                name: "AdditionalScore",
                schema: "characters",
                table: "ability_scores",
                newName: "additional_score");

            migrationBuilder.RenameColumn(
                name: "component_id",
                schema: "characters",
                table: "ability_scores",
                newName: "parent_component_id");

            migrationBuilder.RenameIndex(
                name: "IX_ability_scores_component_id",
                schema: "characters",
                table: "ability_scores",
                newName: "IX_ability_scores_parent_component_id");

            migrationBuilder.RenameIndex(
                name: "IX_ability_scores_AssociatedRegistrationId",
                schema: "characters",
                table: "ability_scores",
                newName: "IX_ability_scores_associated_registration_id");

            migrationBuilder.RenameIndex(
                name: "IX_character_progression_parent_character",
                schema: "characters",
                table: "component_progression",
                newName: "IX_component_progression_parent_character");

            migrationBuilder.RenameIndex(
                name: "IX_character_component_skills_parent_character",
                schema: "characters",
                table: "component_skills",
                newName: "IX_component_skills_parent_character");

            migrationBuilder.RenameIndex(
                name: "IX_character_component_saving_throws_parent_character",
                schema: "characters",
                table: "component_saving_throws",
                newName: "IX_component_saving_throws_parent_character");

            migrationBuilder.RenameIndex(
                name: "IX_character_component_class_parent_character",
                schema: "characters",
                table: "component_class",
                newName: "IX_component_class_parent_character");

            migrationBuilder.RenameIndex(
                name: "IX_character_component_appearance_parent_character",
                schema: "characters",
                table: "component_appearance",
                newName: "IX_component_appearance_parent_character");

            migrationBuilder.RenameIndex(
                name: "IX_character_component_abilities_parent_character",
                schema: "characters",
                table: "component_abilities",
                newName: "IX_component_abilities_parent_character");

            migrationBuilder.AddPrimaryKey(
                name: "PK_component_progression",
                schema: "characters",
                table: "component_progression",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_component_skills",
                schema: "characters",
                table: "component_skills",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_component_saving_throws",
                schema: "characters",
                table: "component_saving_throws",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_component_class",
                schema: "characters",
                table: "component_class",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_component_appearance",
                schema: "characters",
                table: "component_appearance",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_component_abilities",
                schema: "characters",
                table: "component_abilities",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ability_scores_component_abilities_parent_component_id",
                schema: "characters",
                table: "ability_scores",
                column: "parent_component_id",
                principalSchema: "characters",
                principalTable: "component_abilities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_class_component_class_parent_component_id",
                schema: "characters",
                table: "character_class",
                column: "parent_component_id",
                principalSchema: "characters",
                principalTable: "component_class",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_component_abilities_character_parent_character",
                schema: "characters",
                table: "component_abilities",
                column: "parent_character",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_component_appearance_character_parent_character",
                schema: "characters",
                table: "component_appearance",
                column: "parent_character",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_component_class_character_parent_character",
                schema: "characters",
                table: "component_class",
                column: "parent_character",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_component_progression_character_parent_character",
                schema: "characters",
                table: "component_progression",
                column: "parent_character",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_component_saving_throws_character_parent_character",
                schema: "characters",
                table: "component_saving_throws",
                column: "parent_character",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_component_skills_character_parent_character",
                schema: "characters",
                table: "component_skills",
                column: "parent_character",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_registration_include_rules_registration_parent_registration_id",
                schema: "characters",
                table: "registration_include_rules",
                column: "parent_registration_id",
                principalSchema: "characters",
                principalTable: "registration",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_registration_selection_rules_registration_parent_registration_id",
                schema: "characters",
                table: "registration_selection_rules",
                column: "parent_registration_id",
                principalSchema: "characters",
                principalTable: "registration",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_registration_statistic_rules_registration_parent_registration_id",
                schema: "characters",
                table: "registration_statistic_rules",
                column: "parent_registration_id",
                principalSchema: "characters",
                principalTable: "registration",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_savingthrows_component_saving_throws_parent_component_id",
                schema: "characters",
                table: "savingthrows",
                column: "parent_component_id",
                principalSchema: "characters",
                principalTable: "component_saving_throws",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_skills_component_skills_parent_component_id",
                schema: "characters",
                table: "skills",
                column: "parent_component_id",
                principalSchema: "characters",
                principalTable: "component_skills",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ability_scores_component_abilities_parent_component_id",
                schema: "characters",
                table: "ability_scores");

            migrationBuilder.DropForeignKey(
                name: "FK_character_class_component_class_parent_component_id",
                schema: "characters",
                table: "character_class");

            migrationBuilder.DropForeignKey(
                name: "FK_component_abilities_character_parent_character",
                schema: "characters",
                table: "component_abilities");

            migrationBuilder.DropForeignKey(
                name: "FK_component_appearance_character_parent_character",
                schema: "characters",
                table: "component_appearance");

            migrationBuilder.DropForeignKey(
                name: "FK_component_class_character_parent_character",
                schema: "characters",
                table: "component_class");

            migrationBuilder.DropForeignKey(
                name: "FK_component_progression_character_parent_character",
                schema: "characters",
                table: "component_progression");

            migrationBuilder.DropForeignKey(
                name: "FK_component_saving_throws_character_parent_character",
                schema: "characters",
                table: "component_saving_throws");

            migrationBuilder.DropForeignKey(
                name: "FK_component_skills_character_parent_character",
                schema: "characters",
                table: "component_skills");

            migrationBuilder.DropForeignKey(
                name: "FK_registration_include_rules_registration_parent_registration_id",
                schema: "characters",
                table: "registration_include_rules");

            migrationBuilder.DropForeignKey(
                name: "FK_registration_selection_rules_registration_parent_registration_id",
                schema: "characters",
                table: "registration_selection_rules");

            migrationBuilder.DropForeignKey(
                name: "FK_registration_statistic_rules_registration_parent_registration_id",
                schema: "characters",
                table: "registration_statistic_rules");

            migrationBuilder.DropForeignKey(
                name: "FK_savingthrows_component_saving_throws_parent_component_id",
                schema: "characters",
                table: "savingthrows");

            migrationBuilder.DropForeignKey(
                name: "FK_skills_component_skills_parent_component_id",
                schema: "characters",
                table: "skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_component_skills",
                schema: "characters",
                table: "component_skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_component_saving_throws",
                schema: "characters",
                table: "component_saving_throws");

            migrationBuilder.DropPrimaryKey(
                name: "PK_component_progression",
                schema: "characters",
                table: "component_progression");

            migrationBuilder.DropPrimaryKey(
                name: "PK_component_class",
                schema: "characters",
                table: "component_class");

            migrationBuilder.DropPrimaryKey(
                name: "PK_component_appearance",
                schema: "characters",
                table: "component_appearance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_component_abilities",
                schema: "characters",
                table: "component_abilities");

            migrationBuilder.RenameTable(
                name: "component_skills",
                schema: "characters",
                newName: "character_component_skills",
                newSchema: "characters");

            migrationBuilder.RenameTable(
                name: "component_saving_throws",
                schema: "characters",
                newName: "character_component_saving_throws",
                newSchema: "characters");

            migrationBuilder.RenameTable(
                name: "component_progression",
                schema: "characters",
                newName: "character_progression",
                newSchema: "characters");

            migrationBuilder.RenameTable(
                name: "component_class",
                schema: "characters",
                newName: "character_component_class",
                newSchema: "characters");

            migrationBuilder.RenameTable(
                name: "component_appearance",
                schema: "characters",
                newName: "character_component_appearance",
                newSchema: "characters");

            migrationBuilder.RenameTable(
                name: "component_abilities",
                schema: "characters",
                newName: "character_component_abilities",
                newSchema: "characters");

            migrationBuilder.RenameColumn(
                name: "name",
                schema: "characters",
                table: "skills",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "characters",
                table: "skills",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "calculated_bonus",
                schema: "characters",
                table: "skills",
                newName: "CalculatedBonus");

            migrationBuilder.RenameColumn(
                name: "associated_registration_id",
                schema: "characters",
                table: "skills",
                newName: "AssociatedRegistrationId");

            migrationBuilder.RenameColumn(
                name: "additional_bonus",
                schema: "characters",
                table: "skills",
                newName: "AdditionalBonus");

            migrationBuilder.RenameColumn(
                name: "ability_score_modifier",
                schema: "characters",
                table: "skills",
                newName: "AbilityScoreModifier");

            migrationBuilder.RenameColumn(
                name: "ability_score_id",
                schema: "characters",
                table: "skills",
                newName: "AbilityScoreId");

            migrationBuilder.RenameColumn(
                name: "ability_score_abbreviation",
                schema: "characters",
                table: "skills",
                newName: "AbilityScoreAbbreviation");

            migrationBuilder.RenameColumn(
                name: "parent_component_id",
                schema: "characters",
                table: "skills",
                newName: "component_id");

            migrationBuilder.RenameIndex(
                name: "IX_skills_parent_component_id",
                schema: "characters",
                table: "skills",
                newName: "IX_skills_component_id");

            migrationBuilder.RenameIndex(
                name: "IX_skills_associated_registration_id",
                schema: "characters",
                table: "skills",
                newName: "IX_skills_AssociatedRegistrationId");

            migrationBuilder.RenameColumn(
                name: "name",
                schema: "characters",
                table: "savingthrows",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "characters",
                table: "savingthrows",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "calculated_bonus",
                schema: "characters",
                table: "savingthrows",
                newName: "CalculatedBonus");

            migrationBuilder.RenameColumn(
                name: "associated_registration_id",
                schema: "characters",
                table: "savingthrows",
                newName: "AssociatedRegistrationId");

            migrationBuilder.RenameColumn(
                name: "additional_bonus",
                schema: "characters",
                table: "savingthrows",
                newName: "AdditionalBonus");

            migrationBuilder.RenameColumn(
                name: "ability_score_modifier",
                schema: "characters",
                table: "savingthrows",
                newName: "AbilityScoreModifier");

            migrationBuilder.RenameColumn(
                name: "ability_score_id",
                schema: "characters",
                table: "savingthrows",
                newName: "AbilityScoreId");

            migrationBuilder.RenameColumn(
                name: "ability_score_abbreviation",
                schema: "characters",
                table: "savingthrows",
                newName: "AbilityScoreAbbreviation");

            migrationBuilder.RenameColumn(
                name: "parent_component_id",
                schema: "characters",
                table: "savingthrows",
                newName: "component_id");

            migrationBuilder.RenameIndex(
                name: "IX_savingthrows_parent_component_id",
                schema: "characters",
                table: "savingthrows",
                newName: "IX_savingthrows_component_id");

            migrationBuilder.RenameIndex(
                name: "IX_savingthrows_associated_registration_id",
                schema: "characters",
                table: "savingthrows",
                newName: "IX_savingthrows_AssociatedRegistrationId");

            migrationBuilder.RenameColumn(
                name: "value",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "name",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "parent_registration_id",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "ParentRegistrationId");

            migrationBuilder.RenameColumn(
                name: "associated_statistic_rule_id",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "AssociatedStatisticRuleId");

            migrationBuilder.RenameIndex(
                name: "IX_registration_statistic_rules_parent_registration_id",
                schema: "characters",
                table: "registration_statistic_rules",
                newName: "IX_registration_statistic_rules_ParentRegistrationId");

            migrationBuilder.RenameColumn(
                name: "name",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "parent_registration_id",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "ParentRegistrationId");

            migrationBuilder.RenameColumn(
                name: "element_type",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "ElementType");

            migrationBuilder.RenameColumn(
                name: "current_selection",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "CurrentSelection");

            migrationBuilder.RenameColumn(
                name: "associated_selection_rule_id",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "AssociatedSelectionRuleId");

            migrationBuilder.RenameIndex(
                name: "IX_registration_selection_rules_parent_registration_id",
                schema: "characters",
                table: "registration_selection_rules",
                newName: "IX_registration_selection_rules_ParentRegistrationId");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "characters",
                table: "registration_include_rules",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "parent_registration_id",
                schema: "characters",
                table: "registration_include_rules",
                newName: "ParentRegistrationId");

            migrationBuilder.RenameColumn(
                name: "included_element_name",
                schema: "characters",
                table: "registration_include_rules",
                newName: "IncludedElementName");

            migrationBuilder.RenameColumn(
                name: "included_element_id",
                schema: "characters",
                table: "registration_include_rules",
                newName: "IncludedElementId");

            migrationBuilder.RenameColumn(
                name: "associated_include_rule_id",
                schema: "characters",
                table: "registration_include_rules",
                newName: "AssociatedIncludeRuleId");

            migrationBuilder.RenameIndex(
                name: "IX_registration_include_rules_parent_registration_id",
                schema: "characters",
                table: "registration_include_rules",
                newName: "IX_registration_include_rules_ParentRegistrationId");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "characters",
                table: "registration",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "parent_registration_id",
                schema: "characters",
                table: "registration",
                newName: "ParentRegistrationId");

            migrationBuilder.RenameColumn(
                name: "character_id",
                schema: "characters",
                table: "registration",
                newName: "CharacterId");

            migrationBuilder.RenameColumn(
                name: "associated_element_type",
                schema: "characters",
                table: "registration",
                newName: "AssociatedElementType");

            migrationBuilder.RenameColumn(
                name: "associated_element_name",
                schema: "characters",
                table: "registration",
                newName: "AssociatedElementName");

            migrationBuilder.RenameColumn(
                name: "associated_element_id",
                schema: "characters",
                table: "registration",
                newName: "AssociatedElementId");

            migrationBuilder.RenameIndex(
                name: "IX_registration_character_id",
                schema: "characters",
                table: "registration",
                newName: "IX_registration_CharacterId");

            migrationBuilder.RenameColumn(
                name: "name",
                schema: "characters",
                table: "character_class",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "level",
                schema: "characters",
                table: "character_class",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "characters",
                table: "character_class",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "registration_id",
                schema: "characters",
                table: "character_class",
                newName: "Registration");

            migrationBuilder.RenameColumn(
                name: "is_primary",
                schema: "characters",
                table: "character_class",
                newName: "IsPrimary");

            migrationBuilder.RenameColumn(
                name: "parent_component_id",
                schema: "characters",
                table: "character_class",
                newName: "component_id");

            migrationBuilder.RenameIndex(
                name: "IX_character_class_registration_id",
                schema: "characters",
                table: "character_class",
                newName: "IX_character_class_Registration");

            migrationBuilder.RenameIndex(
                name: "IX_character_class_parent_component_id",
                schema: "characters",
                table: "character_class",
                newName: "IX_character_class_component_id");

            migrationBuilder.RenameColumn(
                name: "name",
                schema: "characters",
                table: "character",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "characters",
                table: "character",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "name",
                schema: "characters",
                table: "ability_scores",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "abbreviation",
                schema: "characters",
                table: "ability_scores",
                newName: "Abbreviation");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "characters",
                table: "ability_scores",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "calculated_score",
                schema: "characters",
                table: "ability_scores",
                newName: "CalculatedScore");

            migrationBuilder.RenameColumn(
                name: "calculated_modifier",
                schema: "characters",
                table: "ability_scores",
                newName: "CalculatedModifier");

            migrationBuilder.RenameColumn(
                name: "base_score",
                schema: "characters",
                table: "ability_scores",
                newName: "BaseScore");

            migrationBuilder.RenameColumn(
                name: "associated_registration_id",
                schema: "characters",
                table: "ability_scores",
                newName: "AssociatedRegistrationId");

            migrationBuilder.RenameColumn(
                name: "additional_score",
                schema: "characters",
                table: "ability_scores",
                newName: "AdditionalScore");

            migrationBuilder.RenameColumn(
                name: "parent_component_id",
                schema: "characters",
                table: "ability_scores",
                newName: "component_id");

            migrationBuilder.RenameIndex(
                name: "IX_ability_scores_parent_component_id",
                schema: "characters",
                table: "ability_scores",
                newName: "IX_ability_scores_component_id");

            migrationBuilder.RenameIndex(
                name: "IX_ability_scores_associated_registration_id",
                schema: "characters",
                table: "ability_scores",
                newName: "IX_ability_scores_AssociatedRegistrationId");

            migrationBuilder.RenameIndex(
                name: "IX_component_skills_parent_character",
                schema: "characters",
                table: "character_component_skills",
                newName: "IX_character_component_skills_parent_character");

            migrationBuilder.RenameIndex(
                name: "IX_component_saving_throws_parent_character",
                schema: "characters",
                table: "character_component_saving_throws",
                newName: "IX_character_component_saving_throws_parent_character");

            migrationBuilder.RenameIndex(
                name: "IX_component_progression_parent_character",
                schema: "characters",
                table: "character_progression",
                newName: "IX_character_progression_parent_character");

            migrationBuilder.RenameIndex(
                name: "IX_component_class_parent_character",
                schema: "characters",
                table: "character_component_class",
                newName: "IX_character_component_class_parent_character");

            migrationBuilder.RenameIndex(
                name: "IX_component_appearance_parent_character",
                schema: "characters",
                table: "character_component_appearance",
                newName: "IX_character_component_appearance_parent_character");

            migrationBuilder.RenameIndex(
                name: "IX_component_abilities_parent_character",
                schema: "characters",
                table: "character_component_abilities",
                newName: "IX_character_component_abilities_parent_character");

            migrationBuilder.AddColumn<Guid>(
                name: "CharacterId",
                schema: "characters",
                table: "skills",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CharacterId",
                schema: "characters",
                table: "savingthrows",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ClassComponentId",
                schema: "characters",
                table: "character_class",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CharacterId",
                schema: "characters",
                table: "ability_scores",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_character_component_skills",
                schema: "characters",
                table: "character_component_skills",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_character_component_saving_throws",
                schema: "characters",
                table: "character_component_saving_throws",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_character_progression",
                schema: "characters",
                table: "character_progression",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_character_component_class",
                schema: "characters",
                table: "character_component_class",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_character_component_appearance",
                schema: "characters",
                table: "character_component_appearance",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_character_component_abilities",
                schema: "characters",
                table: "character_component_abilities",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_skills_CharacterId",
                schema: "characters",
                table: "skills",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_savingthrows_CharacterId",
                schema: "characters",
                table: "savingthrows",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_character_class_ClassComponentId",
                schema: "characters",
                table: "character_class",
                column: "ClassComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ability_scores_CharacterId",
                schema: "characters",
                table: "ability_scores",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ability_scores_character_component_abilities_component_id",
                schema: "characters",
                table: "ability_scores",
                column: "component_id",
                principalSchema: "characters",
                principalTable: "character_component_abilities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_class_character_component_class_component_id",
                schema: "characters",
                table: "character_class",
                column: "component_id",
                principalSchema: "characters",
                principalTable: "character_component_class",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_component_abilities_character_parent_character",
                schema: "characters",
                table: "character_component_abilities",
                column: "parent_character",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_component_appearance_character_parent_character",
                schema: "characters",
                table: "character_component_appearance",
                column: "parent_character",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_component_class_character_parent_character",
                schema: "characters",
                table: "character_component_class",
                column: "parent_character",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_component_saving_throws_character_parent_character",
                schema: "characters",
                table: "character_component_saving_throws",
                column: "parent_character",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_component_skills_character_parent_character",
                schema: "characters",
                table: "character_component_skills",
                column: "parent_character",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_progression_character_parent_character",
                schema: "characters",
                table: "character_progression",
                column: "parent_character",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_registration_include_rules_registration_ParentRegistrationId",
                schema: "characters",
                table: "registration_include_rules",
                column: "ParentRegistrationId",
                principalSchema: "characters",
                principalTable: "registration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_registration_selection_rules_registration_ParentRegistrationId",
                schema: "characters",
                table: "registration_selection_rules",
                column: "ParentRegistrationId",
                principalSchema: "characters",
                principalTable: "registration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_registration_statistic_rules_registration_ParentRegistrationId",
                schema: "characters",
                table: "registration_statistic_rules",
                column: "ParentRegistrationId",
                principalSchema: "characters",
                principalTable: "registration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_savingthrows_character_component_saving_throws_component_id",
                schema: "characters",
                table: "savingthrows",
                column: "component_id",
                principalSchema: "characters",
                principalTable: "character_component_saving_throws",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_skills_character_component_skills_component_id",
                schema: "characters",
                table: "skills",
                column: "component_id",
                principalSchema: "characters",
                principalTable: "character_component_skills",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
