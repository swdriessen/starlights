using Microsoft.Extensions.DependencyInjection;
using Starlights.Platform.Eventing;

namespace Starlights.Platform.Components.Data.EntityFramework;

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
        // TODO: refactor this / use a more efficient approach to resolve handlers

        // each handler is resolved in it's own scope to ensure scoped dependencies are isolated (e.g. persistence contexts)
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlerTypeCollection = typeof(IEnumerable<>).MakeGenericType(handlerType); // to support multiple handlers of the same event type

        // find the concrete handlers for a specific event type
        using var discoveryScope = _scopeFactory.CreateScope();
        var discoveredHandlers = (discoveryScope.ServiceProvider.GetService(handlerTypeCollection) as IEnumerable<object> ?? []).ToList();

        var tasks = new List<Task>(discoveredHandlers.Count);

        for (var index = 0; index < discoveredHandlers.Count; index++)
        {
            var handlerIndex = index; // capture for closure
            tasks.Add(InvokeInOwnScope(handlerIndex));
        }

        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }

        async Task InvokeInOwnScope(int handlerIndex)
        {
            using var handlerScope = _scopeFactory.CreateScope();
            var scopedHandlers = (handlerScope.ServiceProvider.GetService(handlerTypeCollection) as IEnumerable<object> ?? []).ToList();

            object handler = scopedHandlers[handlerIndex];

            var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
            if (handleMethod != null)
            {
                var task = (Task)handleMethod.Invoke(handler, [domainEvent])!;
                await task.ConfigureAwait(false);
            }
        }
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
