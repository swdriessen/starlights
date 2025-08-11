using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;
using Starlights.Platform.Eventing;

namespace Modules.Characters.Services.EventHandlers;

public sealed class AbilityScoreCreatedEventHandler : IDomainEventHandler<AbilityScoreCreatedEvent>, IDomainEventHandler<AbilityScoreUpdatedEvent>
{
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;

    public AbilityScoreCreatedEventHandler(IPersistence persistence, IElementsModuleQueries elements)
    {
        _persistence = persistence;
        _elements = elements;
    }

    public async Task HandleAsync(AbilityScoreUpdatedEvent raisedEvent)
    {
        await UpdateSkills(raisedEvent.AbilityScoreId, new(raisedEvent.CharacterId));


    }

    public async Task HandleAsync(AbilityScoreCreatedEvent raisedEvent)
    {
        await UpdateSkills(raisedEvent.AbilityScoreId, new(raisedEvent.CharacterId));
    }

    private async Task UpdateSkills(AbilityScoreId abilityScoreId, CharacterId characterId)
    {
        using var a = CharactersInstrumentation.StartActivity($"{nameof(AbilityScoreCreatedEventHandler)} | {abilityScoreId} ({characterId})");

        // get character
        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(characterId) ?? throw new InvalidOperationException($"Character with ID {characterId} not found.");


        if (!character.Skills.Any(x => x.AbilityScoreId == Guid.Empty))
        {
            // no skills without an ability score, so we can skip this
            return;
        }

        var registrations = _persistence.GetRepository<IRegistrationRepository>();

        var abilityScore = character.AbilityScores.SingleOrDefault(x => x.Id == abilityScoreId)
            ?? throw new InvalidOperationException($"Ability score with ID {abilityScoreId} not found in character {character.Id}.");

        var abilityRegistration = await registrations.GetRegistrationAsync(new(abilityScore.AssociatedRegistrationId))
            ?? throw new InvalidOperationException($"Ability registration with ID {abilityScore.AssociatedRegistrationId} not found.");


        foreach (var existingSkill in character.Skills.Where(x => x.AbilityScoreId == Guid.Empty))
        {
            // get the registration for the skill
            var skillRegistration = await registrations.GetRegistrationAsync(new(existingSkill.AssociatedRegistrationId))
                ?? throw new InvalidOperationException($"Skill registration with ID {existingSkill.AssociatedRegistrationId} not found.");

            // get the associated element for the skill
            var associatedElement = await _elements.GetSkillModel(skillRegistration.AssociatedElementId)
                ?? throw new InvalidOperationException($"Associated element with ID {skillRegistration.AssociatedElementId} not found.");

            if (associatedElement.PrimaryAbilityElementId == abilityRegistration.AssociatedElementId)
            {
                // this ability score is the primary ability score for the skill
                existingSkill.WithAbilityScore(abilityScore.Id, abilityScore.Abbreviation);
                existingSkill.UpdateAbilityScoreModifier(abilityScore.CalculatedModifier);
            }
        }

        var affectedRows = await _persistence.SaveChangesAsync();
        a?.AddTag("affectedRows", affectedRows);
    }
}


