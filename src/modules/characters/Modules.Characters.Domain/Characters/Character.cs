using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters.Eventing;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Platform.Domain;

namespace Starlights.Modules.Characters.Domain.Characters;

/// <summary>
/// Represents a character in the system.
/// </summary>
[DebuggerDisplay("Id = {Id}, Name = {Name}")]
public sealed class Character : AggregateRoot<CharacterId>
{
    private readonly List<AbilityScore> _abilityScores = [];

    private Character(string name)
        : base(CharacterId.New())
    {
        Name = name;
    }

    /// <summary>
    /// Gets the name of the character.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the collection of ability scores associated with the character.
    /// </summary>
    public IReadOnlyCollection<AbilityScore> AbilityScores => _abilityScores.AsReadOnly();

    /// <summary>
    /// Creates a new instance of the <see cref="Character"/> class with the specified name.
    /// </summary>
    public static Character Create(string name)
    {
        var newCharacter = new Character(name);
        newCharacter.AddDomainEvent(new CharacterCreatedEvent() { CharacterId = newCharacter.Id });
        return newCharacter;
    }

    /// <summary>
    /// Creates a new ability score for the character.
    /// </summary>
    public AbilityScore CreateAbilityScore(RegistrationId associatedRegistrationId, string name, string abbreviation)
    {
        var abilityScore = AbilityScore.Create(associatedRegistrationId, name, abbreviation);
        _abilityScores.Add(abilityScore);

        AddDomainEvent(new AbilityScoreCreatedEvent() { CharacterId = Id, AbilityScoreId = abilityScore.Id });

        return abilityScore;
    }
}
