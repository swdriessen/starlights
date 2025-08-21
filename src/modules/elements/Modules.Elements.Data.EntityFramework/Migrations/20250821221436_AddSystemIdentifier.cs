using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Elements.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemIdentifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "requirements",
                schema: "elements",
                table: "element_component_prerequisites",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "system_identifier",
                schema: "elements",
                table: "element",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "requirements",
                schema: "elements",
                table: "element_component_prerequisites");

            migrationBuilder.DropColumn(
                name: "system_identifier",
                schema: "elements",
                table: "element");
        }
    }
}
