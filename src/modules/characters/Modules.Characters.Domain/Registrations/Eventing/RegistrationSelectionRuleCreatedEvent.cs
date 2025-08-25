namespace Starlights.Modules.Characters.Domain.Registrations.Eventing;

public record RegistrationSelectionRuleCreatedEvent : RegistrationEventBase
{
    public Guid ParentRegistrationId => RegistrationId;
    public required Guid RegistrationSelectionRuleId { get; init; }
    public required string ElementType { get; init; }
    public required string Name { get; init; }
}
