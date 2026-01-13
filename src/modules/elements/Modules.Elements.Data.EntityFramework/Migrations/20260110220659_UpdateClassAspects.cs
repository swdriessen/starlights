using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Elements.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClassAspects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "element_component_aspect_class",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    hd = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_aspect_class", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_aspect_class_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_aspect_feature",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<int>(type: "int", nullable: false),
                    listing_order = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_aspect_feature", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_aspect_feature_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_meta",
                schema: "elements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owning_element_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    component_order_sequence = table.Column<int>(type: "int", nullable: false),
                    parent_element = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_meta", x => x.id);
                    table.ForeignKey(
                        name: "FK_element_component_meta_element_owning_element_id",
                        column: x => x.owning_element_id,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_element_component_aspect_class_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_aspect_class",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_aspect_feature_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_aspect_feature",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_element_component_meta_owning_element_id_component_order_sequence",
                schema: "elements",
                table: "element_component_meta",
                columns: new[] { "owning_element_id", "component_order_sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "element_component_aspect_class",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_aspect_feature",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_meta",
                schema: "elements");
        }
    }
}
