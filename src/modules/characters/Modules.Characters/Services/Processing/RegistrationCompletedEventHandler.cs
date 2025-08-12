using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Services.Processing;

public sealed class RegistrationCompletedEventHandler : IDomainEventHandler<RegistrationCreatedEvent>
{
    private readonly IRegistrationManager _registrationManager;

    public RegistrationCompletedEventHandler(IRegistrationManager registrationManager)
    {
        _registrationManager = registrationManager;
    }

    public async Task HandleAsync(RegistrationCreatedEvent raisedEvent)
    {
        using var _ = CharactersInstrumentation.StartActivity($"{nameof(RegistrationCompletedEventHandler)} | {raisedEvent.AssociatedElementName} ({raisedEvent.AssociatedElementType})");

        // raise specific domain events...
        // e.g. if the registration is for an ability, we can raise an AbilityRegistrationCompletedEvent


        //new AbilityRegistrationCompletedEvent
        //{
        //    CharacterId = r.CharacterId,
        //    RegistrationId = r.Id,
        //    AssociatedElementName = r.AssociatedElementName,
        //    AssociatedElementType = r.AssociatedElementType
        //}



        await _registrationManager.ProcessRegistration(new(raisedEvent.RegistrationId));
    }
}
