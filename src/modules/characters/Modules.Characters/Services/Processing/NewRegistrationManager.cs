using System;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Services.Processing;

public class NewRegistrationManager : INewRegistrationManager
{
    private readonly ILogger<NewRegistrationManager> _logger;
    private readonly IPersistence _persistence;
    private readonly List<IRegistrationBehavior> _registrationBehaviors;

    public NewRegistrationManager(ILogger<NewRegistrationManager> logger, IPersistence persistence, IEnumerable<IRegistrationBehavior> registrationBehaviors)
    {
        _logger = logger;
        _persistence = persistence;
        _registrationBehaviors = [.. registrationBehaviors];
    }

    public async Task Register(Registration newRegistration, RegistrationProcessContext context)
    {
        _logger.LogInformation("Registering new registration '{ElementName} ({ElementType})' [character='{CharacterId}']", 
            newRegistration.AssociatedElementName, newRegistration.AssociatedElementType, newRegistration.CharacterId);

        foreach (var behavior in _registrationBehaviors)
        {
            await behavior.Registered(newRegistration, context);
        }
    }

    public async Task Unregister(Registration existingRegistration)
    {
        _logger.LogInformation("Unregistering registration '{ElementName} ({ElementType})' [character='{CharacterId}']", 
            existingRegistration.AssociatedElementName, existingRegistration.AssociatedElementType, existingRegistration.CharacterId);

        foreach (var behavior in _registrationBehaviors)
        {
            await behavior.Unregister(existingRegistration);
        }
    }
}