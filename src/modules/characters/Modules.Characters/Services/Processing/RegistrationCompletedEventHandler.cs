using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Platform.Eventing;

namespace Modules.Characters.Services.Processing;

public sealed class RegistrationCompletedEventHandler : IDomainEventHandler<RegistrationCreated>
{
    private readonly IRegistrationManager _registrationManager;

    public RegistrationCompletedEventHandler(IRegistrationManager registrationManager)
    {
        _registrationManager = registrationManager;
    }

    public async Task HandleAsync(RegistrationCreated raisedEvent)
    {
        using var _ = CharactersInstrumentation.StartActivity($"{nameof(RegistrationCompletedEventHandler)} | {raisedEvent.AssociatedElementName} ({raisedEvent.AssociatedElementType})");
        await _registrationManager.ProcessRegistration(new(raisedEvent.RegistrationId));
    }
}
