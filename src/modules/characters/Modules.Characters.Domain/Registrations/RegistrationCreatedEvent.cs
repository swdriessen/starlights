using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Domain.Registrations;

public record RegistrationCreatedEvent : IDomainEvent
{
    public DateTime OccurredOn => DateTime.UtcNow;

    public required Guid RegistrationId { get; init; }
}