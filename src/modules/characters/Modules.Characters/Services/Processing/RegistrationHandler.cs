using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations;
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

    public async Task HandleAsync(RegistrationCreatedEvent domainEvent)
    {
        using var _ = CharactersInstrumentation.StartActivity("HandleRegistrationCreatedEvent");
        await _registrationManager.ProcessRegistration(new(domainEvent.RegistrationId));
    }
}