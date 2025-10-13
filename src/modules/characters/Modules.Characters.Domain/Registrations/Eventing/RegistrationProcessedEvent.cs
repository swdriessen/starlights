namespace Starlights.Modules.Characters.Domain.Registrations.Eventing;

public record RegistrationProcessedEvent : RegistrationEventBase
{
    public required string ElementName { get; init; }
    public required string ElementType { get; init; }
}
