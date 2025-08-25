using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteCascades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_registration_selection_rules_registration_RegistrationId",
                schema: "characters",
                table: "registration_selection_rules");

            migrationBuilder.DropForeignKey(
                name: "FK_registration_statistic_rules_registration_RegistrationId",
                schema: "characters",
                table: "registration_statistic_rules");

            migrationBuilder.DropIndex(
                name: "IX_registration_statistic_rules_RegistrationId",
                schema: "characters",
                table: "registration_statistic_rules");

            migrationBuilder.DropIndex(
                name: "IX_registration_selection_rules_RegistrationId",
                schema: "characters",
                table: "registration_selection_rules");

            migrationBuilder.DropColumn(
                name: "RegistrationId",
                schema: "characters",
                table: "registration_statistic_rules");

            migrationBuilder.DropColumn(
                name: "RegistrationId",
                schema: "characters",
                table: "registration_selection_rules");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_registration_selection_rules_registration_ParentRegistrationId",
                schema: "characters",
                table: "registration_selection_rules");

            migrationBuilder.DropForeignKey(
                name: "FK_registration_statistic_rules_registration_ParentRegistrationId",
                schema: "characters",
                table: "registration_statistic_rules");

            migrationBuilder.AddColumn<Guid>(
                name: "RegistrationId",
                schema: "characters",
                table: "registration_statistic_rules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RegistrationId",
                schema: "characters",
                table: "registration_selection_rules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_registration_statistic_rules_RegistrationId",
                schema: "characters",
                table: "registration_statistic_rules",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_registration_selection_rules_RegistrationId",
                schema: "characters",
                table: "registration_selection_rules",
                column: "RegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_registration_selection_rules_registration_RegistrationId",
                schema: "characters",
                table: "registration_selection_rules",
                column: "RegistrationId",
                principalSchema: "characters",
                principalTable: "registration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_registration_statistic_rules_registration_RegistrationId",
                schema: "characters",
                table: "registration_statistic_rules",
                column: "RegistrationId",
                principalSchema: "characters",
                principalTable: "registration",
                principalColumn: "Id");
        }
    }
}
