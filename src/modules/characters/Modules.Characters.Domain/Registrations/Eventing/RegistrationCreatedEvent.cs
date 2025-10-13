namespace Starlights.Modules.Characters.Domain.Registrations.Eventing;

public record RegistrationCreatedEvent : RegistrationEventBase
{
    public required string AssociatedElementName { get; init; }
    public required string AssociatedElementType { get; init; }
}

public record RegistrationDeletedEvent : RegistrationEventBase
{
    public required string AssociatedElementName { get; init; }
    public required string AssociatedElementType { get; init; }
}
