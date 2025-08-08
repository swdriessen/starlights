using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Platform.Eventing.EventPublisher;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DomainEventPublisher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    /// <inheritdoc />
    public async Task PublishAsync(IDomainEvent domainEvent)
    {
        // Handlers are registered as IDomainEventHandler<T> (wrappers create a scope per call)
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlerTypeCollection = typeof(IEnumerable<>).MakeGenericType(handlerType); // to support multiple handlers of the same event type

        using var discoveryScope = _scopeFactory.CreateScope();
        var discoveredHandlers = (discoveryScope.ServiceProvider.GetService(handlerTypeCollection) as IEnumerable<object> ?? []).ToList();

        if (discoveredHandlers.Count == 0)
        {
            return;
        }

        var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
        if (handleMethod is null)
        {
            return;
        }

        var tasks = new List<Task>(discoveredHandlers.Count);
        foreach (var handler in discoveredHandlers)
        {
            var task = (Task)handleMethod.Invoke(handler, new object[] { domainEvent })!;
            tasks.Add(task);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        var tasks = new List<Task>();
        foreach (var domainEvent in domainEvents)
        {
            // Use a separate scope per event to ensure isolation of scoped dependencies across events
            tasks.Add(PublishAsync(domainEvent));
        }

        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }
}
