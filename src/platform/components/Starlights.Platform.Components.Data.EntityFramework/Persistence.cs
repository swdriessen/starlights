using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Starlights.Platform.Data;

namespace Starlights.Platform.Components.Data.EntityFramework;

public class Persistence : IPersistence
{
    private readonly ILogger<Persistence> _logger;
    private readonly IContextFactoryRegistry _contextFactoryRegistry;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, IPersistenceContext> _contexts = [];
    private bool _disposedValue;

    public Persistence(ILogger<Persistence> logger, IContextFactoryRegistry contextFactoryRegistry, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _contextFactoryRegistry = contextFactoryRegistry;
        _serviceProvider = serviceProvider;
    }

    public T GetRepository<T>() where T : IRepository
    {
        using var _ = PersistenceTelemetry.ActivitySource.StartActivity($"GetRepository ({typeof(T).Name})", ActivityKind.Internal);

        _logger.LogInformation("get repository [type='{RepositoryType}']", typeof(T).Name);

        var repository = _serviceProvider.GetRequiredService<T>();

        // Get the appropriate context factory for this repository type
        var contextFactory = _contextFactoryRegistry.GetContextFactory<T>();
        var contextType = contextFactory.ContextType; // Use the ContextType property - no runtime context creation!

        // Reuse existing context or create new one for this context type
        if (!_contexts.TryGetValue(contextType, out var persistenceContext))
        {
            _logger.LogInformation("creating a new context instance [context='{ContextType}']", contextType.Name);
            persistenceContext = contextFactory.CreateContext();
            _contexts[contextType] = persistenceContext;
        }

        _logger.LogInformation("setting persistence context for repository [repository='{RepositoryType}', context='{ContextType}']", typeof(T).Name, contextType.Name);

        repository.SetPersistenceContext(persistenceContext);

        return repository;
    }

    public async Task<int> SaveChangesAsync()
    {
        using var activity = PersistenceTelemetry.ActivitySource.StartActivity("SaveChangesAsync", ActivityKind.Internal);

        var totalChanges = 0;

        foreach (var (contextType, context) in _contexts)
        {
            if (context is DbContext dbContext)
            {
                _logger.LogInformation("saving context [type='{ContextType}']", contextType.Name);
                var changes = await dbContext.SaveChangesAsync();
                totalChanges += changes;
                _logger.LogInformation("...saved successfully [rows='{Rows}', context='{ContextType}']", changes, contextType.Name);
            }
        }

        activity?.AddTag("totalChanges", totalChanges);

        return totalChanges;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                foreach (var context in _contexts.Values)
                {
                    if (context is DbContext dbContext)
                    {
                        dbContext.Dispose();
                    }
                }
                _contexts.Clear();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
