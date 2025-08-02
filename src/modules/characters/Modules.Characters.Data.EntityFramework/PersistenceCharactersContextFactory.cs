using Microsoft.EntityFrameworkCore;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Data.EntityFramework;

internal class PersistenceCharactersContextFactory : IPersistenceContextFactory
{
    private readonly IDbContextFactory<CharactersContext> _factory;

    public Type ContextType => typeof(CharactersContext);

    public PersistenceCharactersContextFactory(IDbContextFactory<CharactersContext> factory)
    {
        _factory = factory;
    }

    public IPersistenceContext CreateContext()
    {
        return _factory.CreateDbContext();
    }
}