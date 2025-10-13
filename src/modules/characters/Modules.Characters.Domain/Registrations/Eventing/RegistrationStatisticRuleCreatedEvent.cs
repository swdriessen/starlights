namespace Starlights.Modules.Characters.Domain.Registrations.Eventing;

public record RegistrationStatisticRuleCreatedEvent : RegistrationEventBase
{
    public Guid ParentRegistrationId => RegistrationId;
    public required Guid RegistrationStatisticRuleId { get; init; }
    public required string Name { get; init; }
    public required string Value { get; init; }
}

public record RegistrationStatisticRuleDeletedEvent : RegistrationEventBase
{
    public Guid ParentRegistrationId => RegistrationId;
    public required Guid RegistrationStatisticRuleId { get; init; }
    public required string Name { get; init; }
    public required string Value { get; init; }
}
