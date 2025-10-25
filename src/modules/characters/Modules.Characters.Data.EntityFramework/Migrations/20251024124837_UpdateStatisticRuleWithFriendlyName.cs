using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStatisticRuleWithFriendlyName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "friendly_name",
                schema: "characters",
                table: "registration_statistic_rules",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "friendly_name",
                schema: "characters",
                table: "registration_statistic_rules");
        }
    }
}
