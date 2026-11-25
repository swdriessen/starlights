using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Starlights.Modules.FileManagement.Data;

internal sealed class DesignTimeFileManagementContextFactory : IDesignTimeDbContextFactory<FileManagementContext>
{
    public FileManagementContext CreateDbContext(string[] args)
        => new(new DbContextOptionsBuilder<FileManagementContext>()
            .UseSqlServer(string.Empty, sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "file_management")).Options);
}
