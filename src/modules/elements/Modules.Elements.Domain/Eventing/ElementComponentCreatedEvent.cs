namespace Starlights.Modules.Elements.Domain.Eventing;

public record ElementComponentCreatedEvent(Guid ElementId, Guid ElementComponentId, string ComponentName) : ElementEventBase(ElementId);
