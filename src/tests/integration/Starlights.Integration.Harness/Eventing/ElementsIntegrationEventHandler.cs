using Starlights.Modules.Elements.Domain.Eventing;
using Starlights.Platform.Eventing;

namespace Starlights.Integration.Eventing;

internal sealed class ElementsIntegrationEventHandler :
    IDomainEventHandler<ElementCreatedEvent>
{
    private readonly EventObserverCollection _observers;

    public ElementsIntegrationEventHandler(EventObserverCollection observers)
    {
        _observers = observers;
    }

    public Task HandleAsync(ElementCreatedEvent domainEvent)
    {
        return _observers.HandleAsync(domainEvent);
    }
}
