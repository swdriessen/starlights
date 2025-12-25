namespace Starlights.Modules.Elements.Domain;

public record ElementCreatedEvent(Guid ElementId, string Name, string Type) : ElementEventBase(ElementId);
