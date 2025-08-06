using Microsoft.Extensions.DependencyInjection;
using Starlights.Platform.Eventing;

namespace Starlights.Platform.Components.Data.EntityFramework;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public async Task PublishAsync(IDomainEvent domainEvent)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = _serviceProvider.GetServices(handlerType);

        var tasks = new List<Task>();
        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
            if (handleMethod != null)
            {
                var task = (Task)handleMethod.Invoke(handler, [domainEvent])!;
                tasks.Add(task);
            }
        }

        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }

    /// <inheritdoc />
    public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        var tasks = new List<Task>();
        foreach (var domainEvent in domainEvents)
        {
            tasks.Add(PublishAsync(domainEvent));
        }

        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }
}
