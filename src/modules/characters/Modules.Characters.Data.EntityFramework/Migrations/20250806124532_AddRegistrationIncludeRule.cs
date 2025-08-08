using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationIncludeRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ElementName",
                schema: "characters",
                table: "registration",
                newName: "AssociatedElementName");

            migrationBuilder.RenameColumn(
                name: "ElementId",
                schema: "characters",
                table: "registration",
                newName: "AssociatedElementId");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentRegistrationId",
                schema: "characters",
                table: "registration",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "registration_include_rules",
                schema: "characters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssociatedIncludeRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncludedElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncludedElementName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registration_include_rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_registration_include_rules_registration_RegistrationId",
                        column: x => x.RegistrationId,
                        principalSchema: "characters",
                        principalTable: "registration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_registration_include_rules_RegistrationId",
                schema: "characters",
                table: "registration_include_rules",
                column: "RegistrationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registration_include_rules",
                schema: "characters");

            migrationBuilder.DropColumn(
                name: "ParentRegistrationId",
                schema: "characters",
                table: "registration");

            migrationBuilder.RenameColumn(
                name: "AssociatedElementName",
                schema: "characters",
                table: "registration",
                newName: "ElementName");

            migrationBuilder.RenameColumn(
                name: "AssociatedElementId",
                schema: "characters",
                table: "registration",
                newName: "ElementId");
        }
    }
}
