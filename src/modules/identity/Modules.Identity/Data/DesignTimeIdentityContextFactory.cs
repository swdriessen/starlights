using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Starlights.Modules.Identity.Data;

internal sealed class DesignTimeIdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
{
    public IdentityContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<IdentityContext>()
            .UseSqlServer(string.Empty, sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "identity"))
            .Options;
        return new IdentityContext(options);
    }
}
