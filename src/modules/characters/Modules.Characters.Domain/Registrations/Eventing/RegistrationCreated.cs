namespace Starlights.Modules.Characters.Domain.Registrations.Eventing;

public record RegistrationCreated : RegistrationEventBase
{
    public required string AssociatedElementName { get; init; }
}
