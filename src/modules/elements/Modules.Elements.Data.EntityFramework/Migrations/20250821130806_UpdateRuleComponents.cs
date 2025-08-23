using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Elements.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRuleComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "elements",
                table: "element_component_statistic_rule",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                schema: "elements",
                table: "element_component_statistic_rule",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                schema: "elements",
                table: "element_component_include_rule",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "element_component_selection_rule",
                schema: "elements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwningElement = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ElementType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Supports = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RangeSupports = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LevelRequirement = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_selection_rule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_element_component_selection_rule_element_OwningElement",
                        column: x => x.OwningElement,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_element_component_selection_rule_OwningElement",
                schema: "elements",
                table: "element_component_selection_rule",
                column: "OwningElement");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "element_component_selection_rule",
                schema: "elements");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "elements",
                table: "element_component_statistic_rule");

            migrationBuilder.DropColumn(
                name: "Requirements",
                schema: "elements",
                table: "element_component_statistic_rule");

            migrationBuilder.DropColumn(
                name: "Requirements",
                schema: "elements",
                table: "element_component_include_rule");
        }
    }
}
