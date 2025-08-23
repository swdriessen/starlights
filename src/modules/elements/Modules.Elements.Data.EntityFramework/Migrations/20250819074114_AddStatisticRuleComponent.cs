using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Elements.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddStatisticRuleComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "element_component_statistic_rule",
                schema: "elements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwningElement = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StackingBonus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LevelRequirement = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_statistic_rule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_element_component_statistic_rule_element_OwningElement",
                        column: x => x.OwningElement,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_element_component_statistic_rule_OwningElement",
                schema: "elements",
                table: "element_component_statistic_rule",
                column: "OwningElement");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "element_component_statistic_rule",
                schema: "elements");
        }
    }
}
