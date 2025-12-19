using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Elements.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddSpellAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "elements");

            migrationBuilder.CreateTable(
                name: "element",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    system_identifier = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "element_component_abbreviation",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    abbreviation = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_abbreviation", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_abbreviation_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_ability",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_ability", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_ability_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_description",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_description", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_description_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_include_rule",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    include_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    level_requirement = table.Column<int>(type: "int", nullable: false),
                    requirements = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_include_rule", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_include_rule_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_language",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    origin = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    kind = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_language", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_language_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_prerequisites",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    prerequisites = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    requirements = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_prerequisites", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_prerequisites_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_primary_ability",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    primary_ability_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_primary_ability", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_primary_ability_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_selection_rule",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    element_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    short_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    supports = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    range_supports = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    requirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    level_requirement = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    is_optional = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_selection_rule", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_selection_rule_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_short_description",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_short_description", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_short_description_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_sorting",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    sorting_order = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_sorting", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_sorting_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_spell_attributes",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<int>(type: "int", nullable: false),
                    magic_school = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    casting_time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    range = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_concentration_required = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_ritual = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    has_somatic_component = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    has_verbal_component = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    has_material_component = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    material_component = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "element_component_statistic_rule",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    display_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    stacking_bonus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    level_requirement = table.Column<int>(type: "int", nullable: false),
                    requirements = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_statistic_rule", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_statistic_rule_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_element_component_abbreviation_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_abbreviation",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_ability_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_ability",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_description_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_description",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_include_rule_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_include_rule",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_language_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_language",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_prerequisites_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_prerequisites",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_primary_ability_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_primary_ability",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_selection_rule_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_selection_rule",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_short_description_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_short_description",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_sorting_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_sorting",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_spell_attributes_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_spell_attributes",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_statistic_rule_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_statistic_rule",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "element_component_abbreviation",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_ability",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_description",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_include_rule",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_language",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_prerequisites",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_primary_ability",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_selection_rule",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_short_description",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_sorting",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_spell_attributes",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_statistic_rule",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element",
                schema: "elements");
        }
    }
}
