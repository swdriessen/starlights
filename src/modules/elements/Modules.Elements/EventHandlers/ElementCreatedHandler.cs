using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Domain.Eventing;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Elements.EventHandlers;

public class ElementCreatedHandler : IDomainEventHandler<ElementCreatedEvent>, IDomainEventHandler<ElementComponentCreatedEvent>
{
    private readonly ILogger<ElementCreatedHandler> _logger;

    public ElementCreatedHandler(ILogger<ElementCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ElementCreatedEvent domainEvent)
    {
        //_logger.LogInformation("⌛ element created: {ElementName} ({ElementType}) [id={ElementId}]", domainEvent.Name, domainEvent.Type, domainEvent.ElementId);
        return Task.CompletedTask;
    }

    public Task HandleAsync(ElementComponentCreatedEvent domainEvent)
    {
        //_logger.LogDebug("⌛   component created: {ComponentName} [id={ElementId}]", domainEvent.ComponentName, domainEvent.ElementId);
        return Task.CompletedTask;
    }
}
