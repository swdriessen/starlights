using Starlights.Modules.Elements.Domain.Eventing;
using Starlights.Platform.Eventing;

namespace Starlights.Integration.Eventing;

internal sealed class ElementsIntegrationEventHandler :
    IDomainEventHandler<ElementCreatedEvent>
{
    private readonly ElementsEventObserverCollection _observers;

    public ElementsIntegrationEventHandler(ElementsEventObserverCollection observers)
    {
        _observers = observers;
    }

    public Task HandleAsync(ElementCreatedEvent domainEvent)
    {
        return _observers.ElementCreated.Mock.Object.HandleAsync(domainEvent);
    }
}
