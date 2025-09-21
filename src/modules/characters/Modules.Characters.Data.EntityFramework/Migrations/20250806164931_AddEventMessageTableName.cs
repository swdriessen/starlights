using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.Characters.Data.EntityFramework.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddEventMessageTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EventMessage",
                schema: "characters",
                table: "EventMessage");

            migrationBuilder.RenameTable(
                name: "EventMessage",
                schema: "characters",
                newName: "event_messages",
                newSchema: "characters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_event_messages",
                schema: "characters",
                table: "event_messages",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_event_messages",
                schema: "characters",
                table: "event_messages");

            migrationBuilder.RenameTable(
                name: "event_messages",
                schema: "characters",
                newName: "EventMessage",
                newSchema: "characters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventMessage",
                schema: "characters",
                table: "EventMessage",
                column: "Id");
        }
    }
}
