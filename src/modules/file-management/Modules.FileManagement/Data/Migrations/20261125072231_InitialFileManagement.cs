using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Starlights.Modules.FileManagement.Data.Migrations;

/// <inheritdoc />
public partial class _20261125072231_InitialFileManagement : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "file_management");

        migrationBuilder.CreateTable(
            name: "files",
            schema: "file_management",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                IsDeleting = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_files", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_files_OwnerId_CreatedAt_Id",
            schema: "file_management",
            table: "files",
            columns: new[] { "OwnerId", "CreatedAt", "Id" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "files",
            schema: "file_management");
    }
}
