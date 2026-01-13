namespace Starlights.Modules.Elements.Domain.Eventing;

public record ElementCreatedEvent(Guid ElementId, string Name, string Type) : ElementEventBase(ElementId);
