using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Registrations.Eventing;

public record RegistrationEventBase : CharacterEventBase
{
    public required Guid RegistrationId { get; init; }
}
