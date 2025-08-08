using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Modules.Characters.Services.Processing;

public class RegistrationManager : IRegistrationManager
{
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;

    public RegistrationManager(IPersistence persistence, IElementsModuleQueries elements)
    {
        _persistence = persistence;
        _elements = elements;
    }

    public async Task<int> ProcessRegistration(RegistrationId registrationId)
    {
        using var processActivity = CharactersInstrumentation.StartActivity();

        var registrations = _persistence.GetRepository<IRegistrationRepository>();
        var registration = await registrations.GetRegistrationAsync(registrationId);

        if (registration is null)
        {
            throw new InvalidOperationException($"Registration with ID {registrationId} not found.");
        }

        var context = new RegistrationProcessContext(registrations, registration);

        // step 1 process the registration, this can be a pipeline of steps
        await ProcessIncludeRules(context);

        // step 2 process registration hooks
        // lets first keep it simple and hook into the events

        // step 3 mark as processed (raise domain event)
        registration.Processed();

        // step 4 save the registration
        var affectedRows = await _persistence.SaveChangesAsync();

        processActivity?.SetTag("affectedRows", affectedRows);

        return affectedRows;
    }

    private async Task ProcessIncludeRules(RegistrationProcessContext context)
    {
        using var _ = CharactersInstrumentation.StartActivity();

        var currentRegistration = context.Registration;

        var currentElement = await _elements.GetElementWithRules(currentRegistration.AssociatedElementId);

        // if the current element has no include rules, we can skip processing

        foreach (var rule in currentElement.IncludeRules)
        {
            if (currentRegistration.HasAssociatedRule(rule.RuleId))
            {
                continue;
            }

            // when all requirements are met, we can include the element
            var newIncludeElement = await _elements.GetElementWithRules(rule.IncludedElementId);

            // create the new registration for the included element
            var newRegistration = Registration.Create(currentRegistration.CharacterId, new(newIncludeElement.Id), newIncludeElement.Name);
            newRegistration.UpdateParentRegistration(currentRegistration);

            // create the new registration include rule, this is to keep track of the rules applied
            currentRegistration.CreateIncludeRule(new(rule.RuleId), new(newIncludeElement.Id), newIncludeElement.Name);

            context.Repository.Add(newRegistration);
        }
    }
}
