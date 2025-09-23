using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations;

/// <inheritdoc />
public partial class AddEventMessage : Migration
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "EventMessage",
            schema: "characters",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OccurredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                ProcessedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EventMessage", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "EventMessage",
            schema: "characters");
    }
}
