using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Elements.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddProficiencyAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "element_component_proficiency_attributes",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    proficiency_type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_proficiency_attributes", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_proficiency_attributes_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_element_component_proficiency_attributes_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_proficiency_attributes",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "element_component_proficiency_attributes",
                schema: "elements");
        }
    }
}
