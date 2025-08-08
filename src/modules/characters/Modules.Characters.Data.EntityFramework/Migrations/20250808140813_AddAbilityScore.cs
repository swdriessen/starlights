using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddAbilityScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ability_scores",
                schema: "characters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssociatedRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseScore = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    CharacterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ability_scores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ability_scores_character_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "characters",
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ability_scores_AssociatedRegistrationId",
                schema: "characters",
                table: "ability_scores",
                column: "AssociatedRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_ability_scores_CharacterId",
                schema: "characters",
                table: "ability_scores",
                column: "CharacterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ability_scores",
                schema: "characters");
        }
    }
}
