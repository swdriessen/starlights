using Microsoft.EntityFrameworkCore;

namespace Starlights.Modules.FileManagement.Data;

internal sealed class FileManagementContext(DbContextOptions<FileManagementContext> options) : DbContext(options)
{
    public DbSet<StoredFile> Files => Set<StoredFile>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("file_management");
        builder.ApplyConfiguration(new StoredFileConfiguration());
    }
}
