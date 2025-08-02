using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Starlights.Modules.Characters.Data.EntityFramework;

[ExcludeFromCodeCoverage]
internal sealed class DesignTimeCharactersContextFactory : IDesignTimeDbContextFactory<CharactersContext>
{
    public CharactersContext CreateDbContext(string[] args)
    {
        string connectionString = "";
        if (args.Length > 0)
        {
            // If arguments are provided, use the first one as the connection string
            connectionString = args[0];
        }

        var builder = new DbContextOptionsBuilder<CharactersContext>();
        builder.UseSqlServer(connectionString);

        return new CharactersContext(builder.Options);
    }
}
