using Microsoft.EntityFrameworkCore;
using Starlights.Platform.Components.Data.EntityFramework;
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
        modelBuilder.HasDefaultSchema("elements");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ElementsContext).Assembly);

        // create extension method for this...
        modelBuilder.ApplyConfiguration(new EventMessageTypeConfiguration());
        modelBuilder.Entity<EventMessage>().ToTable("event_messages");
    }
}
