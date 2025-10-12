using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Progression.Eventing;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Modules.Characters.Services.Processing;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Services.EventHandlers;

public sealed class ReproccessingEventHandler : IDomainEventHandler<RegistrationDeletedEvent>, IDomainEventHandler<CharacterLevelChangedEvent>
{
    private readonly ILogger<ReproccessingEventHandler> _logger;
    private readonly IRegistrationProcessor _registrationProcessor;

    public ReproccessingEventHandler(ILogger<ReproccessingEventHandler> logger, IRegistrationProcessor registrationProcessor)
    {
        _logger = logger;
        _registrationProcessor = registrationProcessor;
    }

    public async Task HandleAsync(RegistrationDeletedEvent domainEvent)
    {
        using var activity = CharactersInstrumentation.StartActivity($"{nameof(RegistrationDeletedEvent)} | {domainEvent.AssociatedElementName} ({domainEvent.AssociatedElementType})");

        try
        {
            await _registrationProcessor.ReproccessRegistrations(new(domainEvent.CharacterId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reprocessing registrations for character {CharacterId} after deletion of registration {RegistrationId}", domainEvent.CharacterId, domainEvent.RegistrationId);
            activity?.AddException(ex);
        }
    }

    public async Task HandleAsync(CharacterLevelChangedEvent domainEvent)
    {
        using var activity = CharactersInstrumentation.StartActivity($"{nameof(CharacterLevelChangedEvent)} | Level {domainEvent.NewLevel}");

        try
        {
            await _registrationProcessor.ReproccessRegistrations(new(domainEvent.CharacterId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reprocessing registrations for character {CharacterId} after level change to {NewLevel}", domainEvent.CharacterId, domainEvent.NewLevel);
            activity?.AddException(ex);
        }
    }
}
