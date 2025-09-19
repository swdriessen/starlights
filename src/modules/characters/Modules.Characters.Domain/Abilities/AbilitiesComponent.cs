using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Components;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Domain.Abilities;

public sealed class AbilitiesComponent : CharacterComponentBase
{
    private readonly List<AbilityScore> _abilityScores = [];

    public AbilitiesComponent(CharacterId parentCharacter)
        : base(parentCharacter)
    {
    }

    /// <summary>
    /// Gets the collection of ability scores associated with the character.
    /// </summary>
    public IReadOnlyCollection<AbilityScore> AbilityScores => _abilityScores.AsReadOnly();

    /// <summary>
    /// Creates a new ability score for the character.
    /// </summary>
    public AbilityScore CreateAbilityScore(RegistrationId associatedRegistrationId, string name, string abbreviation)
    {
        var abilityScore = AbilityScore.Create(associatedRegistrationId, name, abbreviation);

        AddDomainEvent(new AbilityScoreCreatedEvent() { CharacterId = ParentCharacter, AbilityScoreId = abilityScore.Id });

        _abilityScores.Add(abilityScore);
        return abilityScore;
    }

    /// <summary>
    /// Updates the base score of an ability score for the character.
    /// </summary>
    public void UpdateAbilityBaseScore(AbilityScoreId abilityScoreId, int value)
    {
        var ability = _abilityScores.SingleOrDefault(a => a.Id == abilityScoreId) ?? throw new InvalidOperationException($"AbilityScore with ID {abilityScoreId} not found for Character {Id}.");

        if (value is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Base score must be between 1 and 20.");
        }

        ability.UpdateBaseScore(value);

        AddDomainEvent(new AbilityScoreUpdatedEvent() { CharacterId = ParentCharacter, AbilityScoreId = ability.Id });
    }

    /// <summary>
    /// Updates the additional score of an ability score for the character.
    /// </summary>
    public void UpdateAbilityAdditionalScore(AbilityScoreId abilityScoreId, int value)
    {
        var ability = _abilityScores.SingleOrDefault(a => a.Id == abilityScoreId) ?? throw new InvalidOperationException($"AbilityScore with ID {abilityScoreId} not found for Character {Id}.");
        ability.UpdateAdditionalScore(value);

        AddDomainEvent(new AbilityScoreUpdatedEvent() { CharacterId = ParentCharacter, AbilityScoreId = ability.Id });
    }

    public static AbilitiesComponent Create(CharacterId parentCharacter)
    {
        return new AbilitiesComponent(parentCharacter);
    }
}
