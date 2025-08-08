namespace Starlights.Modules.Characters.Domain.Registrations.Eventing;

public record RegistrationCreatedEvent : RegistrationEventBase
{
    public required string AssociatedElementName { get; init; }
}
