using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Platform.Eventing;

namespace Modules.Characters.Services.Processing;

public sealed class RegistrationCreatedEventHandler : IDomainEventHandler<RegistrationCreatedEvent>
{
    private readonly ILogger<RegistrationCreatedEventHandler> _logger;
    private readonly IRegistrationManager _registrationManager;

    public RegistrationCreatedEventHandler(ILogger<RegistrationCreatedEventHandler> logger, IRegistrationManager registrationManager)
    {
        _logger = logger;
        _registrationManager = registrationManager;
    }

    public async Task HandleAsync(RegistrationCreatedEvent raisedEvent)
    {
        using var _ = CharactersInstrumentation.StartActivity($"{nameof(RegistrationCreatedEventHandler)} ({raisedEvent.AssociatedElementName})");
        await _registrationManager.ProcessRegistration(new(raisedEvent.RegistrationId));
    }
}