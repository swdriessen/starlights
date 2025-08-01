using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Starlights.Modules.Elements.Data.EntityFramework;

[ExcludeFromCodeCoverage]
internal sealed class DesignTimeElementsContextFactory : IDesignTimeDbContextFactory<ElementsContext>
{
    public ElementsContext CreateDbContext(string[] args)
    {
        string connectionString = "";
        if (args.Length > 0)
        {
            // If arguments are provided, use the first one as the connection string
            connectionString = args[0];
            Console.WriteLine($"Using connection string from args: {connectionString}");
        }
        else
        {
            Console.WriteLine("No connection string provided in args, using default.");
        }

        var builder = new DbContextOptionsBuilder<ElementsContext>();
        builder.UseSqlServer(connectionString);

        return new ElementsContext(builder.Options);
    }
}
