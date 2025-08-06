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
        using var _ = CharactersInstrumentation.StartActivity();

        var registrations = _persistence.GetRepository<IRegistrationRepository>();
        var registration = await registrations.GetRegistrationAsync(registrationId);

        if (registration is null)
        {
            throw new InvalidOperationException($"Registration with ID {registrationId} not found.");
        }

        var context = new RegistrationProcessContext(registrations, registration);

        // process include rules
        await ProcessIncludeRules(context);

        return await _persistence.SaveChangesAsync();
    }

    private async Task ProcessIncludeRules(RegistrationProcessContext context)
    {
        using var _ = CharactersInstrumentation.StartActivity();

        var registration = context.Registration;

        // get full element with rules
        var element = await _elements.GetElementWithRules(registration.AssociatedElementId);

        foreach (var rule in element.IncludeRules)
        {
            if (registration.IncludeRules.Any(r => r.AssociatedIncludeRuleId == rule.RuleId))
            {
                // Skip if the rule already exists
                continue;
            }

            // include meets requirements, create new registration for the included element
            var newIncludeElement = await _elements.GetElementWithRules(rule.IncludedElementId);

            // create the new registration include rule
            registration.CreateIncludeRule(new(rule.RuleId), new(newIncludeElement.Id), newIncludeElement.Name);

            // create the new registration for the included element
            var newRegistration = Registration.Create(registration.CharacterId, new(newIncludeElement.Id), newIncludeElement.Name);
            newRegistration.UpdateParentRegistration(context.Registration);

            context.Repository.Add(newRegistration);

            // when persistence is saved, domain events will process the new registration
        }
    }
}
