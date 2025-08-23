using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationStatisticsRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "registration_statistic_rules",
                schema: "characters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssociatedStatisticRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registration_statistic_rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_registration_statistic_rules_registration_RegistrationId",
                        column: x => x.RegistrationId,
                        principalSchema: "characters",
                        principalTable: "registration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_registration_statistic_rules_ParentRegistrationId",
                schema: "characters",
                table: "registration_statistic_rules",
                column: "ParentRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_registration_statistic_rules_RegistrationId",
                schema: "characters",
                table: "registration_statistic_rules",
                column: "RegistrationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registration_statistic_rules",
                schema: "characters");
        }
    }
}
