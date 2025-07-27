using Microsoft.EntityFrameworkCore;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Data.EntityFramework;

public class ElementsContext : DbContext, IPersistenceContext
{
    public ElementsContext(DbContextOptions<ElementsContext> options)
       : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Console.WriteLine("OnModelCreating: ApplyConfigurationsFromAssembly");
        modelBuilder.HasDefaultSchema("dnd");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ElementsContext).Assembly);
    }
}

//internal sealed class DesignTimeElementsContextFactory : IDesignTimeDbContextFactory<ElementsContext>
//{
//    public ElementsContext CreateDbContext(string[] args)
//    {
//        var builder = new DbContextOptionsBuilder<ElementsContext>();
//        builder.UseSqlServer(string.Empty);

//        return new ElementsContext(builder.Options);
//    }
//}