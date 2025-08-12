using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Services.EventHandlers;

public sealed class AbilityScoreCreatedEventHandler : IDomainEventHandler<AbilityScoreCreatedEvent>
{
    private readonly ILogger<AbilityScoreCreatedEventHandler> _logger;
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;

    public AbilityScoreCreatedEventHandler(ILogger<AbilityScoreCreatedEventHandler> logger, IPersistence persistence, IElementsModuleQueries elements)
    {
        _logger = logger;
        _persistence = persistence;
        _elements = elements;
    }

    public async Task HandleAsync(AbilityScoreCreatedEvent domainEvent)
    {
        var characterId = new CharacterId(domainEvent.CharacterId);
        var abilityScoreId = new AbilityScoreId(domainEvent.AbilityScoreId);

        using var handlerActivity = CharactersInstrumentation.StartActivity($"{nameof(AbilityScoreCreatedEventHandler)} | {abilityScoreId.Value}");
        handlerActivity?.AddTag("characterId", characterId.Value);

        // get character
        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(characterId);
        if (character is null)
        {
            _logger.LogWarning("The character was not found, unable to react to AbilityScoreCreatedEvent. [character='{CharacterId}']", characterId.Value);
            return;
        }

        // check if there are skills that need to be updated
        var unassignedSkills = character.Skills.Where(x => x.AbilityScoreId == Guid.Empty); // once custom skills are implemented, this will need to be adjusted to check for custom skills as well
        if (!unassignedSkills.Any())
        {
            // no skills without an ability score, so we can skip this
            return;
        }

        // get the ability score
        var abilityScore = character.AbilityScores.SingleOrDefault(x => x.Id == abilityScoreId);
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

        foreach (var unassignedSkill in unassignedSkills)
        {
            // get the registration for the skill
            var skillRegistration = await registrations.GetRegistrationAsync(new(unassignedSkill.AssociatedRegistrationId));
            if (skillRegistration is null)
            {
                _logger.LogWarning("The skill registration was not found, unable to react to AbilityScoreCreatedEvent. [skill='{SkillId}']", unassignedSkill.AssociatedRegistrationId);
                continue;
            }

            // get the associated element for the skill
            var skillElement = await _elements.GetSkillModel(skillRegistration.AssociatedElementId);
            if (skillElement is null)
            {
                _logger.LogWarning("The skill element was not found, unable to react to AbilityScoreCreatedEvent. [skill='{SkillId}']", unassignedSkill.AssociatedRegistrationId);
                continue;
            }

            if (skillElement.PrimaryAbilityElementId == abilityRegistration.AssociatedElementId)
            {
                // this ability score is the primary ability score for the skill
                unassignedSkill.WithAbilityScore(abilityScore.Id, abilityScore.Abbreviation);
                unassignedSkill.UpdateAbilityScoreModifier(abilityScore.CalculatedModifier);
            }
        }

        var affectedRows = await _persistence.SaveChangesAsync();
        handlerActivity?.AddTag("affectedRows", affectedRows);
        handlerActivity?.DisplayName = $"{nameof(AbilityScoreCreatedEventHandler)} | {abilityScore.Name} ({characterId.Value})";
    }
}


