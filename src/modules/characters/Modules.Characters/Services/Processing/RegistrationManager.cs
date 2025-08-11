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

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var registrations = _persistence.GetRepository<IRegistrationRepository>();
        var registration = await registrations.GetRegistrationAsync(registrationId) ?? throw new InvalidOperationException($"Registration with ID {registrationId} not found.");

        var context = new RegistrationProcessContext(registration, _persistence);

        // step 1 process the registration, this can be a pipeline of steps
        await ProcessIncludeRules(context);

        // step 2 process registration hooks
        // lets first keep it simple and hook into the events
        // add something like... IIncludeRuleRegistrationBehavior etc

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
            var newRegistration = Registration.Create(currentRegistration.CharacterId, new(newIncludeElement.Id), newIncludeElement.Name, newIncludeElement.Type);
            newRegistration.UpdateParentRegistration(currentRegistration);

            // add specific events based on the type of the included element (TODO: inject something instead)
            newRegistration.IncludeSpecificEvents();

            await new AbilityIncludedBehavior(_elements)
                .Registered(newRegistration, context);

            // create asi / skill here works?

            // create the new registration include rule, this is to keep track of the rules applied
            currentRegistration.CreateIncludeRule(new(rule.RuleId), new(newIncludeElement.Id), newIncludeElement.Name);

            context.GetRepository<IRegistrationRepository>().Add(newRegistration);
        }
    }
}


public interface IIncludeRuleRegistrationBehavior
{
    Task Registered(Registration newRegistration, RegistrationProcessContext context);
}

public sealed class AbilityIncludedBehavior : IIncludeRuleRegistrationBehavior
{
    private readonly IElementsModuleQueries _elements;

    public AbilityIncludedBehavior(IElementsModuleQueries elements)
    {
        _elements = elements;
    }

    public async Task Registered(Registration newRegistration, RegistrationProcessContext context)
    {
        if (newRegistration.AssociatedElementType == "Ability")
        {
            // when a new ability element is registered, we need to create the ability score for the character
            var associatedElement = await _elements.GetAbilityModel(newRegistration.AssociatedElementId) ?? throw new InvalidOperationException($"Ability with ID {newRegistration.AssociatedElementId} not found.");

            // get the character (could be a property in the context, specially if we need character level and other data later for requirements)
            var characters = context.GetRepository<ICharactersRepository>();
            var character = await characters.GetCharacterAsync(newRegistration.CharacterId) ?? throw new InvalidOperationException($"Character with ID {newRegistration.CharacterId} not found.");

            character.CreateAbilityScore(newRegistration.Id, associatedElement.Name, associatedElement.Abbreviation);
        }
    }
}