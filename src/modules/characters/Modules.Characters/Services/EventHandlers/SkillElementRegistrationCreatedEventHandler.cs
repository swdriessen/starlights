using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Elements.Integration;
using Starlights.Modules.Elements.Integration.Models;
using Starlights.Platform.Data;
using Starlights.Platform.Eventing;

namespace Modules.Characters.Services.EventHandlers;

public sealed class SkillElementRegistrationCreatedEventHandler : IDomainEventHandler<SkillElementRegistrationCreatedEvent>
{
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;

    public SkillElementRegistrationCreatedEventHandler(IPersistence persistence, IElementsModuleQueries elements)
    {
        _persistence = persistence;
        _elements = elements;
    }

    public async Task HandleAsync(SkillElementRegistrationCreatedEvent raisedEvent)
    {
        using var _ = CharactersInstrumentation.StartActivity($"{nameof(SkillElementRegistrationCreatedEventHandler)} | {raisedEvent.AssociatedElementName} ({raisedEvent.AssociatedElementType})");

        // get the registration
        var registrations = _persistence.GetRepository<IRegistrationRepository>();
        var registration = await registrations.GetRegistrationAsync(new(raisedEvent.RegistrationId)) ?? throw new InvalidOperationException($"Registration with ID {raisedEvent.RegistrationId} not found.");

        // get character
        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(raisedEvent.CharacterId) ?? throw new InvalidOperationException($"Character with ID {raisedEvent.CharacterId} not found.");

        // get the associated ability element
        var associatedElement = await _elements.GetSkillModel(registration.AssociatedElementId) ?? throw new InvalidOperationException($"Associated element with ID {registration.AssociatedElementId} not found. Ensure it is registered as a skill.");

        // get the registration of the ability score associated with the skill, through the associated element id of the ability
        var primaryScore = await GetPrimaryAbilityScore(registrations, character, associatedElement);

        if (primaryScore is null)
        {
            character.CreateSkillWithoutAbilityScore(registration.Id, associatedElement.Name);
        }
        else
        {
            character.CreateSkill(registration.Id, associatedElement.Name, primaryScore.Id, primaryScore.Abbreviation);
        }

        await _persistence.SaveChangesAsync();
    }

    private static async Task<AbilityScore?> GetPrimaryAbilityScore(IRegistrationRepository registrations, Character character, SkillDataModel skill)
    {
        var all = await registrations.GetRegistrationsAsync(character.Id);

        foreach (var abilityRegistration in all)
        {
            if (abilityRegistration.AssociatedElementId == skill.PrimaryAbilityElementId)
            {
                return character.AbilityScores.SingleOrDefault(a => a.AssociatedRegistrationId == abilityRegistration.Id);
            }
        }

        return null;
    }
}


