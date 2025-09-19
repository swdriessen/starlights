using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.SavingThrows;
using Starlights.Modules.Characters.Domain.Skills;

namespace Starlights.Modules.Characters.Domain.Services;

public interface ICharacterAbilitiesUpdateService
{
    /// <summary>
    /// Updates the base score of a specific ability for a character.
    /// </summary>
    void UpdateAbilityBaseScore(Character character, AbilityScoreId abilityScoreId, int newBaseScore);

    /// <summary>
    /// Updates the additional score of a specific ability for a character.
    /// </summary>
    void UpdateAbilityAdditionalScore(Character character, AbilityScoreId abilityScoreId, int newAdditionalScore);
}

public sealed class CharacterAbilitiesUpdateService : ICharacterAbilitiesUpdateService
{
    public void UpdateAbilityBaseScore(Character character, AbilityScoreId abilityScoreId, int newBaseScore)
    {
        var abilitiesComponent = character.UpdateComponent<AbilitiesComponent>((abilities, _) =>
            abilities.UpdateAbilityBaseScore(abilityScoreId, newBaseScore));

        var updateAbility = abilitiesComponent.AbilityScores.Single(x => x.Id == abilityScoreId);

        character.UpdateComponent<SkillsComponent>((skills, _) =>
            skills.UpdateAbilityScoreModifier(updateAbility.Id, updateAbility.CalculatedModifier));

        character.UpdateComponent<SavingThrowsComponent>((savingThrows, _) =>
            savingThrows.UpdateAbilityScoreModifier(updateAbility.Id, updateAbility.CalculatedModifier));
    }

    public void UpdateAbilityAdditionalScore(Character character, AbilityScoreId abilityScoreId, int newAdditionalScore)
    {
        var abilitiesComponent = character.UpdateComponent<AbilitiesComponent>((abilities, _) =>
            abilities.UpdateAbilityAdditionalScore(abilityScoreId, newAdditionalScore));

        var updateAbility = abilitiesComponent.AbilityScores.Single(x => x.Id == abilityScoreId);

        character.UpdateComponent<SkillsComponent>((skills, _) =>
            skills.UpdateAbilityScoreModifier(updateAbility.Id, updateAbility.CalculatedModifier));

        character.UpdateComponent<SavingThrowsComponent>((savingThrows, _) =>
            savingThrows.UpdateAbilityScoreModifier(updateAbility.Id, updateAbility.CalculatedModifier));
    }
}