using Starlights.Platform.Eventing;

namespace Starlights.Modules.Elements.Domain;

public record ElementEventBase(Guid ElementId) : EventBase;