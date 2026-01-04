using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Elements.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddSpellcastingAspects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "element_component_spell_attributes",
                schema: "elements");

            migrationBuilder.CreateTable(
                name: "element_component_aspects_spellcasting",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    casting_time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    range = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    components = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_aspects_spellcasting", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_aspects_spellcasting_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_element_component_aspects_spellcasting_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_aspects_spellcasting",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "element_component_aspects_spellcasting",
                schema: "elements");

            migrationBuilder.CreateTable(
                name: "element_component_spell_attributes",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    casting_time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    has_material_component = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    has_somatic_component = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    has_verbal_component = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_concentration_required = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_ritual = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    level = table.Column<int>(type: "int", nullable: false),
                    magic_school = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    material_components_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    range = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_spell_attributes", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_spell_attributes_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_element_component_spell_attributes_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_spell_attributes",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);
        }
    }
}
