using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Services.Processing;

public sealed class RegistrationCompletedEventHandler : IDomainEventHandler<RegistrationCreatedEvent>
{
    private readonly IRegistrationProcessor _registrationProcessor;

    public RegistrationCompletedEventHandler(IRegistrationProcessor registrationProcessor)
    {
        _registrationProcessor = registrationProcessor;
    }

    public async Task HandleAsync(RegistrationCreatedEvent raisedEvent)
    {
        using var activity = CharactersInstrumentation.StartActivity($"{nameof(RegistrationCompletedEventHandler)} | {raisedEvent.AssociatedElementName} ({raisedEvent.AssociatedElementType})");
        
        try
        {

            await _registrationProcessor.ProcessRegistration(new(raisedEvent.RegistrationId));
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
        }
    }
}
