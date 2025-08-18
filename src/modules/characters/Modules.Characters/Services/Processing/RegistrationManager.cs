using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Services.Processing;

public class RegistrationManager : IRegistrationManager
{
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;
    private readonly IEnumerable<IRegistrationBehavior> _registrationBehaviors;

    public RegistrationManager(IPersistence persistence, IElementsModuleQueries elements, IEnumerable<IRegistrationBehavior> registrationBehaviors)
    {
        _persistence = persistence;
        _elements = elements;
        _registrationBehaviors = registrationBehaviors;
    }

    public async Task<int> ProcessRegistration(RegistrationId registrationId)
    {
        using var processActivity = CharactersInstrumentation.StartActivity();

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var registrations = _persistence.GetRepository<IRegistrationRepository>();
        var registration = await registrations.GetRegistrationAsync(registrationId) ?? throw new InvalidOperationException($"Registration with ID {registrationId} not found.");

        var context = new RegistrationProcessContext(registration, _persistence);

        await ProcessIncludeRules(context);

        registration.Processed();

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

        var registrations = context.GetRepository<IRegistrationRepository>();

        foreach (var rule in currentElement.IncludeRules)
        {
            if (currentRegistration.HasAssociatedRule(rule.RuleId))
            {
                continue;
            }

            // when all requirements are met, we can include the element
            var newIncludeElement = await _elements.GetElementWithRules(rule.IncludedElementId);

            // create the new registration for the included element
            var newRegistration = Registration.Create(currentRegistration.CharacterId, new(newIncludeElement.Id), newIncludeElement.Name, newIncludeElement.Type);
            newRegistration.UpdateParentRegistration(currentRegistration);

            // create the new registration include rule, this is to keep track of the rules applied
            currentRegistration.CreateIncludeRule(new(rule.RuleId), new(newIncludeElement.Id), newIncludeElement.Name);

            registrations.Add(newRegistration);
            context.NewRegistrations.Add(newRegistration);

            // apply any registration behavior in the current context
            foreach (var behavior in _registrationBehaviors)
            {
                await behavior.Registered(newRegistration, context);
            }
        }
    }
}
