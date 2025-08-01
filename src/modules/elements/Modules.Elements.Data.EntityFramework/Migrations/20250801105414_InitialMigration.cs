using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Elements.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "element_component_abbreviation",
                schema: "elements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwningElement = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_abbreviation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_element_component_abbreviation_element_OwningElement",
                        column: x => x.OwningElement,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_ability",
                schema: "elements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwningElement = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_ability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_element_component_ability_element_OwningElement",
                        column: x => x.OwningElement,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_description",
                schema: "elements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwningElement = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_description", x => x.Id);
                    table.ForeignKey(
                        name: "FK_element_component_description_element_OwningElement",
                        column: x => x.OwningElement,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_include_rule",
                schema: "elements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwningElement = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncludeElement = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LevelRequirement = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_include_rule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_element_component_include_rule_element_OwningElement",
                        column: x => x.OwningElement,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_language",
                schema: "elements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwningElement = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_language", x => x.Id);
                    table.ForeignKey(
                        name: "FK_element_component_language_element_OwningElement",
                        column: x => x.OwningElement,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_prerequisites",
                schema: "elements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwningElement = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Prerequisites = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_prerequisites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_element_component_prerequisites_element_OwningElement",
                        column: x => x.OwningElement,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_primary_ability",
                schema: "elements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwningElement = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrimaryAbility = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_primary_ability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_element_component_primary_ability_element_OwningElement",
                        column: x => x.OwningElement,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_short_description",
                schema: "elements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwningElement = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_short_description", x => x.Id);
                    table.ForeignKey(
                        name: "FK_element_component_short_description_element_OwningElement",
                        column: x => x.OwningElement,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_component_sorting",
                schema: "elements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwningElement = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortingOrder = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_element_component_sorting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_element_component_sorting_element_OwningElement",
                        column: x => x.OwningElement,
                        principalSchema: "elements",
                        principalTable: "element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_element_component_abbreviation_OwningElement",
                schema: "elements",
                table: "element_component_abbreviation",
                column: "OwningElement");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_ability_OwningElement",
                schema: "elements",
                table: "element_component_ability",
                column: "OwningElement");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_description_OwningElement",
                schema: "elements",
                table: "element_component_description",
                column: "OwningElement");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_include_rule_OwningElement",
                schema: "elements",
                table: "element_component_include_rule",
                column: "OwningElement");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_language_OwningElement",
                schema: "elements",
                table: "element_component_language",
                column: "OwningElement");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_prerequisites_OwningElement",
                schema: "elements",
                table: "element_component_prerequisites",
                column: "OwningElement");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_primary_ability_OwningElement",
                schema: "elements",
                table: "element_component_primary_ability",
                column: "OwningElement");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_short_description_OwningElement",
                schema: "elements",
                table: "element_component_short_description",
                column: "OwningElement");

            migrationBuilder.CreateIndex(
                name: "IX_element_component_sorting_OwningElement",
                schema: "elements",
                table: "element_component_sorting",
                column: "OwningElement");
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
                name: "element_component_short_description",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element_component_sorting",
                schema: "elements");

            migrationBuilder.DropTable(
                name: "element",
                schema: "elements");
        }
    }
}
