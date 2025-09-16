using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Components;
using Starlights.Modules.Characters.Domain.Progression;

namespace Starlights.Modules.Characters.Domain.Services;


public interface ICharacterCreationService
{
    /// <summary>
    /// Creates a new character instance.
    /// </summary>
    Character Create(string name);
}

public sealed class CharacterCreationService : ICharacterCreationService
{
    private readonly ILogger<CharacterCreationService> _logger;

    public CharacterCreationService(ILogger<CharacterCreationService> logger)
    {
        _logger = logger;
    }

    public Character Create(string name)
    {
        var newCharacter = Character.Create(name);

        // initialize default components
        newCharacter.AddComponent(new AppearanceComponent(newCharacter.Id));
        newCharacter.AddComponent(new ProgressionComponent(newCharacter.Id));
        newCharacter.AddComponent(new AbilitiesComponent(newCharacter.Id));
        newCharacter.AddComponent(new ClassComponent(newCharacter.Id));

        return newCharacter;
    }
}

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
        character.UpdateComponent<AbilitiesComponent>((abilities, _) =>
        {
            // update ability base score
            abilities.UpdateAbilityBaseScore(abilityScoreId, newBaseScore);
        });
    }

    public void UpdateAbilityAdditionalScore(Character character, AbilityScoreId abilityScoreId, int newAdditionalScore)
    {
        character.UpdateComponent<AbilitiesComponent>((abilities, _) =>
        {
            // update ability additional score
            abilities.UpdateAbilityAdditionalScore(abilityScoreId, newAdditionalScore);
        });
    }
}