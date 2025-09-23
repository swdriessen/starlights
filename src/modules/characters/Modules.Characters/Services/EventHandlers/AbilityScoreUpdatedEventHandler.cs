using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Skills;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Services.EventHandlers;

public sealed class AbilityScoreUpdatedEventHandler : IDomainEventHandler<AbilityScoreUpdatedEvent>
{
    private readonly ILogger<AbilityScoreUpdatedEventHandler> _logger;
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;

    public AbilityScoreUpdatedEventHandler(ILogger<AbilityScoreUpdatedEventHandler> logger, IPersistence persistence, IElementsModuleQueries elements)
    {
        _logger = logger;
        _persistence = persistence;
        _elements = elements;
    }

    public async Task HandleAsync(AbilityScoreUpdatedEvent domainEvent)
    {
        var characterId = new CharacterId(domainEvent.CharacterId);
        var abilityScoreId = new AbilityScoreId(domainEvent.AbilityScoreId);

        using var handlerActivity = CharactersInstrumentation.StartActivity($"{nameof(AbilityScoreUpdatedEventHandler)} | {abilityScoreId.Value}");
        handlerActivity?.AddTag("characterId", characterId.Value);

        // get character
        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(characterId);
        if (character is null)
        {
            _logger.LogWarning("The character was not found, unable to react to AbilityScoreCreatedEvent. [character='{CharacterId}']", characterId.Value);
            return;
        }

        // get the ability score
        var abilities = character.GetRequiredComponent<AbilitiesComponent>();
        var abilityScore = abilities.AbilityScores.SingleOrDefault(x => x.Id == abilityScoreId);
        if (abilityScore is null)
        {
            _logger.LogWarning("The ability score was not found, unable to react to AbilityScoreCreatedEvent. [abilityScore='{AbilityScoreId}']", abilityScoreId.Value);
            return;
        }

        var registrations = _persistence.GetRepository<IRegistrationRepository>();

        var abilityRegistration = await registrations.GetRegistrationAsync(new(abilityScore.AssociatedRegistrationId));
        if (abilityRegistration is null)
        {
            _logger.LogWarning("The ability score registration was not found, unable to react to AbilityScoreCreatedEvent. [abilityScore='{AbilityScoreId}']", abilityScoreId.Value);
            return;
        }

        var skillsComponent = character.GetRequiredComponent<SkillsComponent>();

        foreach (var existingSkill in skillsComponent.Skills)
        {
            if (!existingSkill.HasAssociatedAbilityScore)
            {
                // this skill does not have an ability score assigned, so we can skip it


                // get the registration for the skill
                var skillRegistration = await registrations.GetRegistrationAsync(new(existingSkill.AssociatedRegistrationId));
                if (skillRegistration is null)
                {
                    _logger.LogWarning("The skill registration was not found, unable to react to AbilityScoreCreatedEvent. [skill='{SkillId}']", existingSkill.AssociatedRegistrationId);
                    continue;
                }

                // get the associated element for the skill
                var skillElement = await _elements.GetSkillModel(skillRegistration.AssociatedElementId);
                if (skillElement is null)
                {
                    _logger.LogWarning("The skill element was not found, unable to react to AbilityScoreCreatedEvent. [skill='{SkillId}']", existingSkill.AssociatedRegistrationId);
                    continue;
                }

                if (skillElement.PrimaryAbilityElementId == abilityRegistration.AssociatedElementId)
                {
                    _logger.LogInformation("Assigning ability score '{AbilityScoreId}' to skill '{SkillId}' for character '{CharacterId}'", abilityScore.Id.Value, existingSkill.Id.Value, characterId.Value);
                    // this ability score is the primary ability score for the skill
                    existingSkill.WithAbilityScore(abilityScore.Id, abilityScore.Abbreviation);
                    existingSkill.UpdateAbilityScoreModifier(abilityScore.CalculatedModifier);
                }

                continue;
            }

            // this skill has an ability score assigned, so we need to check if it matches the one we are updating
            if (existingSkill.AbilityScoreId == abilityScoreId.Value)
            {
                // the skill is affected by the update, so we need to update it
                _logger.LogInformation("Updating skill '{SkillId}' for character '{CharacterId}' with new ability score '{AbilityScoreId}'", existingSkill.Id.Value, characterId.Value, abilityScoreId.Value);
                existingSkill.UpdateAbilityScoreModifier(abilityScore.CalculatedModifier);
            }
        }

        var affectedRows = await _persistence.SaveChangesAsync();
        handlerActivity?.AddTag("affectedRows", affectedRows);
        handlerActivity?.DisplayName = $"{nameof(AbilityScoreUpdatedEventHandler)} | {abilityScore.Name} ({characterId.Value})";
    }
}


