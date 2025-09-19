using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCharacterComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ability_scores_character_CharacterId",
                schema: "characters",
                table: "ability_scores");

            migrationBuilder.DropForeignKey(
                name: "FK_character_class_character_component_class_ClassComponentId",
                schema: "characters",
                table: "character_class");

            migrationBuilder.DropForeignKey(
                name: "FK_savingthrows_character_CharacterId",
                schema: "characters",
                table: "savingthrows");

            migrationBuilder.DropForeignKey(
                name: "FK_skills_character_CharacterId",
                schema: "characters",
                table: "skills");

            migrationBuilder.DropTable(
                name: "appearance",
                schema: "characters");

            migrationBuilder.AddColumn<Guid>(
                name: "component_id",
                schema: "characters",
                table: "skills",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "component_id",
                schema: "characters",
                table: "savingthrows",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "component_id",
                schema: "characters",
                table: "character_class",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "component_id",
                schema: "characters",
                table: "ability_scores",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "character_component_abilities",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    parent_character = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_component_abilities", x => x.id);
                    table.ForeignKey(
                        name: "FK_character_component_abilities_character_parent_character",
                        column: x => x.parent_character,
                        principalSchema: "characters",
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_component_appearance",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    parent_character = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PortraitUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_component_appearance", x => x.id);
                    table.ForeignKey(
                        name: "FK_character_component_appearance_character_parent_character",
                        column: x => x.parent_character,
                        principalSchema: "characters",
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_component_saving_throws",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    parent_character = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_component_saving_throws", x => x.id);
                    table.ForeignKey(
                        name: "FK_character_component_saving_throws_character_parent_character",
                        column: x => x.parent_character,
                        principalSchema: "characters",
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_component_skills",
                schema: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    parent_character = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_component_skills", x => x.id);
                    table.ForeignKey(
                        name: "FK_character_component_skills_character_parent_character",
                        column: x => x.parent_character,
                        principalSchema: "characters",
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_skills_component_id",
                schema: "characters",
                table: "skills",
                column: "component_id");

            migrationBuilder.CreateIndex(
                name: "IX_savingthrows_component_id",
                schema: "characters",
                table: "savingthrows",
                column: "component_id");

            migrationBuilder.CreateIndex(
                name: "IX_character_class_component_id",
                schema: "characters",
                table: "character_class",
                column: "component_id");

            migrationBuilder.CreateIndex(
                name: "IX_ability_scores_component_id",
                schema: "characters",
                table: "ability_scores",
                column: "component_id");

            migrationBuilder.CreateIndex(
                name: "IX_character_component_abilities_parent_character",
                schema: "characters",
                table: "character_component_abilities",
                column: "parent_character");

            migrationBuilder.CreateIndex(
                name: "IX_character_component_appearance_parent_character",
                schema: "characters",
                table: "character_component_appearance",
                column: "parent_character");

            migrationBuilder.CreateIndex(
                name: "IX_character_component_saving_throws_parent_character",
                schema: "characters",
                table: "character_component_saving_throws",
                column: "parent_character");

            migrationBuilder.CreateIndex(
                name: "IX_character_component_skills_parent_character",
                schema: "characters",
                table: "character_component_skills",
                column: "parent_character");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "FK_savingthrows_character_component_saving_throws_component_id",
                schema: "characters",
                table: "savingthrows");

            migrationBuilder.DropForeignKey(
                name: "FK_skills_character_component_skills_component_id",
                schema: "characters",
                table: "skills");

            migrationBuilder.DropTable(
                name: "character_component_abilities",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "character_component_appearance",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "character_component_saving_throws",
                schema: "characters");

            migrationBuilder.DropTable(
                name: "character_component_skills",
                schema: "characters");

            migrationBuilder.DropIndex(
                name: "IX_skills_component_id",
                schema: "characters",
                table: "skills");

            migrationBuilder.DropIndex(
                name: "IX_savingthrows_component_id",
                schema: "characters",
                table: "savingthrows");

            migrationBuilder.DropIndex(
                name: "IX_character_class_component_id",
                schema: "characters",
                table: "character_class");

            migrationBuilder.DropIndex(
                name: "IX_ability_scores_component_id",
                schema: "characters",
                table: "ability_scores");

            migrationBuilder.DropColumn(
                name: "component_id",
                schema: "characters",
                table: "skills");

            migrationBuilder.DropColumn(
                name: "component_id",
                schema: "characters",
                table: "savingthrows");

            migrationBuilder.DropColumn(
                name: "component_id",
                schema: "characters",
                table: "character_class");

            migrationBuilder.DropColumn(
                name: "component_id",
                schema: "characters",
                table: "ability_scores");

            migrationBuilder.CreateTable(
                name: "appearance",
                schema: "characters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PortraitUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appearance", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appearance_CharacterId",
                schema: "characters",
                table: "appearance",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ability_scores_character_CharacterId",
                schema: "characters",
                table: "ability_scores",
                column: "CharacterId",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_class_character_component_class_ClassComponentId",
                schema: "characters",
                table: "character_class",
                column: "ClassComponentId",
                principalSchema: "characters",
                principalTable: "character_component_class",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_savingthrows_character_CharacterId",
                schema: "characters",
                table: "savingthrows",
                column: "CharacterId",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_skills_character_CharacterId",
                schema: "characters",
                table: "skills",
                column: "CharacterId",
                principalSchema: "characters",
                principalTable: "character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
