using Microsoft.Extensions.Logging;
using Starlights.Modules.Elements.Domain;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Elements.EventHandlers;

public class ElementCreatedHandler : IDomainEventHandler<ElementCreatedEvent>
{
    private readonly ILogger<ElementCreatedHandler> _logger;

    public ElementCreatedHandler(ILogger<ElementCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ElementCreatedEvent domainEvent)
    {
        _logger.LogInformation("element created: {ElementName} ({ElementType}) [id={ElementId}]", domainEvent.Name, domainEvent.Type, domainEvent.ElementId);
        return Task.CompletedTask;
    }
}
