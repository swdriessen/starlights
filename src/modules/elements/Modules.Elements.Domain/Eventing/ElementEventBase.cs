using Starlights.Platform.Eventing;

namespace Starlights.Modules.Elements.Domain.Eventing;

public record ElementEventBase(Guid ElementId) : EventBase;