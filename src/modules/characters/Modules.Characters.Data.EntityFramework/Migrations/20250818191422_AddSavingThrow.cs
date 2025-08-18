using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddSavingThrow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "savingthrows",
                schema: "characters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssociatedRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AbilityScoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AbilityScoreAbbreviation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AbilityScoreModifier = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AdditionalBonus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CalculatedBonus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CharacterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_savingthrows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_savingthrows_character_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "characters",
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_savingthrows_AssociatedRegistrationId",
                schema: "characters",
                table: "savingthrows",
                column: "AssociatedRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_savingthrows_CharacterId",
                schema: "characters",
                table: "savingthrows",
                column: "CharacterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "savingthrows",
                schema: "characters");
        }
    }
}
