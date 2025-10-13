using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Modules.Characters.Services.Processing;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Services.EventHandlers;

public sealed class ProcessingEventHandler : IDomainEventHandler<RegistrationCreatedEvent>
{
    private readonly ILogger<ProcessingEventHandler> _logger;
    private readonly IRegistrationProcessor _registrationProcessor;

    public ProcessingEventHandler(ILogger<ProcessingEventHandler> logger, IRegistrationProcessor registrationProcessor)
    {
        _logger = logger;
        _registrationProcessor = registrationProcessor;
    }

    public async Task HandleAsync(RegistrationCreatedEvent raisedEvent)
    {
        using var activity = CharactersInstrumentation.StartActivity($"{nameof(RegistrationCreatedEvent)} | {raisedEvent.AssociatedElementName} ({raisedEvent.AssociatedElementType})");

        try
        {
            await _registrationProcessor.ProcessRegistration(new(raisedEvent.RegistrationId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing registration '{RegistrationId}'", raisedEvent.RegistrationId);
            activity?.AddException(ex);
        }
    }
}
