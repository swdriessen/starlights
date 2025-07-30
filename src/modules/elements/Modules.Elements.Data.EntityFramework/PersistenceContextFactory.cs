using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Starlights.Platform.Data;

namespace Starlights.Modules.Elements.Data.EntityFramework;

public class PersistenceContextFactory : IPersistenceContextFactory
{
    private readonly ILogger<PersistenceContextFactory> _logger;
    private readonly IDbContextFactory<ElementsContext> _factory;

    public PersistenceContextFactory(ILogger<PersistenceContextFactory> logger, IDbContextFactory<ElementsContext> factory)
    {
        _logger = logger;
        _factory = factory;
    }

    public IPersistenceContext CreateContext()
    {
        _logger.LogInformation("Creating a new ElementsContext instance.");
        return _factory.CreateDbContext();
    }
}
