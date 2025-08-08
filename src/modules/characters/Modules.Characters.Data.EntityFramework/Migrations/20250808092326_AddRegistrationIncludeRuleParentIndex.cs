using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationIncludeRuleParentIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_registration_include_rules_registration_RegistrationId",
                schema: "characters",
                table: "registration_include_rules");

            migrationBuilder.DropIndex(
                name: "IX_registration_include_rules_RegistrationId",
                schema: "characters",
                table: "registration_include_rules");

            migrationBuilder.DropColumn(
                name: "RegistrationId",
                schema: "characters",
                table: "registration_include_rules");

            migrationBuilder.CreateIndex(
                name: "IX_registration_include_rules_ParentRegistrationId",
                schema: "characters",
                table: "registration_include_rules",
                column: "ParentRegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_registration_include_rules_registration_ParentRegistrationId",
                schema: "characters",
                table: "registration_include_rules",
                column: "ParentRegistrationId",
                principalSchema: "characters",
                principalTable: "registration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_registration_include_rules_registration_ParentRegistrationId",
                schema: "characters",
                table: "registration_include_rules");

            migrationBuilder.DropIndex(
                name: "IX_registration_include_rules_ParentRegistrationId",
                schema: "characters",
                table: "registration_include_rules");

            migrationBuilder.AddColumn<Guid>(
                name: "RegistrationId",
                schema: "characters",
                table: "registration_include_rules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_registration_include_rules_RegistrationId",
                schema: "characters",
                table: "registration_include_rules",
                column: "RegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_registration_include_rules_registration_RegistrationId",
                schema: "characters",
                table: "registration_include_rules",
                column: "RegistrationId",
                principalSchema: "characters",
                principalTable: "registration",
                principalColumn: "Id");
        }
    }
}
