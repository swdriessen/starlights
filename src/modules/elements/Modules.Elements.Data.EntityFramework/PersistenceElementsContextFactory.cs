using Microsoft.EntityFrameworkCore;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Data.EntityFramework;

public class PersistenceElementsContextFactory : IPersistenceContextFactory
{
    private readonly IDbContextFactory<ElementsContext> _factory;

    public Type ContextType => typeof(ElementsContext);

    public PersistenceElementsContextFactory(IDbContextFactory<ElementsContext> factory)
    {
        _factory = factory;
    }

    public IPersistenceContext CreateContext()
    {
        return _factory.CreateDbContext();
    }
}