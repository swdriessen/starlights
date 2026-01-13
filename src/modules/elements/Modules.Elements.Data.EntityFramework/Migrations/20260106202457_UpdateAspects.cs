using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Elements.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAspects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "element_component_ability",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_aspects_spellcasting",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_feat_attributes",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_language",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_proficiency_attributes",
                schema: "elements");

            migrationBuilder.CreateTable(
                name: "element_component_aspect_ability",
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
                    table.PrimaryKey("PK_element_component_aspect_ability", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_aspect_ability_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_aspect_feat",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_aspect_feat", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_aspect_feat_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_aspect_language",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    kind = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    origin = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_aspect_language", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_aspect_language_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_aspect_proficiency",
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
                    table.PrimaryKey("PK_element_component_aspect_proficiency", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_aspect_proficiency_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_aspect_spellcasting",
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
                    table.PrimaryKey("PK_element_component_aspect_spellcasting", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_aspect_spellcasting_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_element_component_aspect_ability_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_aspect_ability",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_aspect_feat_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_aspect_feat",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_aspect_language_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_aspect_language",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_aspect_proficiency_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_aspect_proficiency",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_aspect_spellcasting_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_aspect_spellcasting",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "element_component_aspect_ability",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_aspect_feat",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_aspect_language",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_aspect_proficiency",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_aspect_spellcasting",
                schema: "elements");

            migrationBuilder.CreateTable(
                name: "element_component_ability",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "element_component_aspects_spellcasting",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    casting_time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    components = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    range = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "element_component_feat_attributes",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    category_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_feat_attributes", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_feat_attributes_element_owning_element_id",
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
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    kind = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    origin = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
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
                name: "element_component_proficiency_attributes",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "IX_element_component_ability_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_ability",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_aspects_spellcasting_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_aspects_spellcasting",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_feat_attributes_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_feat_attributes",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_language_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_language",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_proficiency_attributes_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_proficiency_attributes",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);
        }
    }
}
