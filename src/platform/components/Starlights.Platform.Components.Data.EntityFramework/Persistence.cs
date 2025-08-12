using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Starlights.Platform.Data;
using Starlights.Platform.Eventing;

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
        //using var activity = PersistenceInstrumentation.StartActivity($"GetRepository ({typeof(T).Name})");

        var repository = _serviceProvider.GetRequiredService<T>();

        // Get the appropriate context factory for this repository type
        var contextFactory = _contextFactoryRegistry.GetContextFactory<T>();
        var contextType = contextFactory.ContextType; // Use the ContextType property - no runtime context creation!

        // Reuse existing context or create new one for this context type
        if (!_contexts.TryGetValue(contextType, out var persistenceContext))
        {
            _logger.LogDebug("creating a new context instance [context='{ContextType}']", contextType.Name);
            persistenceContext = contextFactory.CreateContext();
            _contexts[contextType] = persistenceContext;
        }

        //_logger.LogDebug("setting persistence context for repository [repository='{RepositoryType}', context='{ContextType}']", typeof(T).Name, contextType.Name);
        repository.SetPersistenceContext(persistenceContext);

        return repository;
    }

    public async Task<int> SaveChangesAsync()
    {
        using var activity = PersistenceInstrumentation.StartActivity();

        var totalChanges = 0;
        var totalEventMessages = 0;

        foreach (var (contextType, context) in _contexts)
        {
            if (context is DbContext database)
            {
                // create the outbox entities for each domain event
                var (domainEntities, domainEvents) = GetEntitiesWithEvents(database);

                if (domainEvents.Count > 0)
                {
                    using var eventsActivity = PersistenceInstrumentation.StartActivity("Store Domain Events");

                    _logger.LogDebug("...found {EventCount} domain events in context '{ContextType}'", domainEvents.Count, contextType.Name);

                    foreach (var domainEvent in domainEvents)
                    {
                        var json = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());
                        var message = EventMessage.Create(domainEvent, domainEvent.GetType(), json);
                        database.Add(message);
                    }

                    domainEntities.ForEach(entity => entity.ClearDomainEvents());
                }
                else
                {
                    _logger.LogDebug("...no domain events found in context '{ContextType}'", contextType.Name);
                }

                var affectedRows = await database.SaveChangesAsync();

                var actualChanges = affectedRows - domainEvents.Count; // subtract the number of domain events saved as outbox messages

                totalChanges += actualChanges;
                totalEventMessages += domainEvents.Count;

                _logger.LogInformation("...saved successfully [rows='{Rows}', events='{EventCount}', context='{ContextType}']", actualChanges, domainEvents.Count, contextType.Name);
            }
        }

        activity?.AddTag("totalChanges", totalChanges);
        activity?.AddTag("totalEventMessages", totalEventMessages);

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
                        _logger.LogDebug("disposing DbContext [type='{ContextType}']", dbContext.GetType().Name);
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

    /// <summary>
    /// Get the entities that have events, and the entire collection of events.
    /// </summary>
    private static (List<IEventEntity> Entities, List<IDomainEvent> Events) GetEntitiesWithEvents(DbContext context)
    {
        var entities = context.ChangeTracker.Entries()
            .Select(entry => entry.Entity)
            .OfType<IEventEntity>()
            .Where(x => x.DomainEvents.Count > 0)
            .ToList();

        var events = entities.SelectMany(entity => entity.DomainEvents).ToList();

        return (entities, events);
    }
}
