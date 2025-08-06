using Microsoft.EntityFrameworkCore;
using Starlights.Platform.Components.Data.EntityFramework;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Data.EntityFramework;

public class CharactersContext : DbContext, IPersistenceContext
{
    public CharactersContext(DbContextOptions<CharactersContext> options)
       : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("characters");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CharactersContext).Assembly);

        // create extension method for this...
        modelBuilder.ApplyConfiguration(new EventMessageTypeConfiguration());
        modelBuilder.Entity<EventMessage>().ToTable("event_messages");
    }
}
