using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Progression.Eventing;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Modules.Characters.Services.Processing;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Services.EventHandlers;

public sealed class RegistrationCompletedEventHandler :
    IDomainEventHandler<RegistrationCreatedEvent>,
    IDomainEventHandler<RegistrationDeletedEvent>,
    IDomainEventHandler<CharacterLevelChangedEvent>
{
    private readonly ILogger<RegistrationCompletedEventHandler> _logger;
    private readonly IRegistrationProcessor _registrationProcessor;

    public RegistrationCompletedEventHandler(ILogger<RegistrationCompletedEventHandler> logger, IRegistrationProcessor registrationProcessor)
    {
        _logger = logger;
        _registrationProcessor = registrationProcessor;
    }

    public async Task HandleAsync(RegistrationCreatedEvent raisedEvent)
    {
        using var activity = CharactersInstrumentation.StartActivity($"{nameof(RegistrationCreatedEvent)}Handler | {raisedEvent.AssociatedElementName} ({raisedEvent.AssociatedElementType})");

        try
        {
            await _registrationProcessor.ProcessRegistration(new(raisedEvent.RegistrationId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing registration {RegistrationId}", raisedEvent.RegistrationId);
            activity?.AddException(ex);
            throw;
        }
    }

    public async Task HandleAsync(RegistrationDeletedEvent domainEvent)
    {
        using var activity = CharactersInstrumentation.StartActivity($"{nameof(RegistrationDeletedEvent)}Handler | {domainEvent.AssociatedElementName} ({domainEvent.AssociatedElementType})");

        try
        {
            await _registrationProcessor.ReproccessRegistrations(new(domainEvent.CharacterId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reprocessing registrations for character {CharacterId} after deletion of registration {RegistrationId}", domainEvent.CharacterId, domainEvent.RegistrationId);
            activity?.AddException(ex);
            throw;
        }
    }

    public async Task HandleAsync(CharacterLevelChangedEvent domainEvent)
    {
        using var activity = CharactersInstrumentation.StartActivity($"{nameof(CharacterLevelChangedEvent)}Handler | New Level {domainEvent.NewLevel}");

        try
        {
            await _registrationProcessor.ReproccessRegistrations(new(domainEvent.CharacterId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reprocessing registrations for character {CharacterId} after level change to {NewLevel}", domainEvent.CharacterId, domainEvent.NewLevel);
            activity?.AddException(ex);
            throw;
        }
    }
}
