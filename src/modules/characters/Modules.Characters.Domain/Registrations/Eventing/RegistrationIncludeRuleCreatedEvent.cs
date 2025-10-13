using Starlights.Modules.Characters.Domain.Elements;

namespace Starlights.Modules.Characters.Domain.Registrations.Eventing;

public record RegistrationIncludeRuleCreatedEvent : RegistrationEventBase
{
    public Guid ParentRegistrationId => RegistrationId;
    public required Guid RegistrationIncludeRuleId { get; init; }
    public required ElementId ElementId { get; init; }
    public required string Name { get; init; }
}

public record RegistrationIncludeRuleDeletedEvent : RegistrationEventBase
{
    public Guid ParentRegistrationId => RegistrationId;
    public required Guid RegistrationIncludeRuleId { get; init; }
    public required ElementId ElementId { get; init; }
    public required string Name { get; init; }
}