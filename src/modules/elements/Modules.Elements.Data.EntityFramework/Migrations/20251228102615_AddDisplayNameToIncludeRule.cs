using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Elements.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddDisplayNameToIncludeRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "elements",
                table: "element_component_include_rule",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "elements",
                table: "element_component_include_rule");
        }
    }
}
