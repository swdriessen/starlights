using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddSelectionRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "registration_selection_rules",
                schema: "characters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssociatedSelectionRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ElementType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registration_selection_rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_registration_selection_rules_registration_RegistrationId",
                        column: x => x.RegistrationId,
                        principalSchema: "characters",
                        principalTable: "registration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_registration_selection_rules_ParentRegistrationId",
                schema: "characters",
                table: "registration_selection_rules",
                column: "ParentRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_registration_selection_rules_RegistrationId",
                schema: "characters",
                table: "registration_selection_rules",
                column: "RegistrationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registration_selection_rules",
                schema: "characters");
        }
    }
}
