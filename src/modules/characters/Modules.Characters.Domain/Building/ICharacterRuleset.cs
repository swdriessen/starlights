namespace Starlights.Modules.Characters.Domain.Building;

/// <summary>Initializes and validates the composition belonging to one Ruleset.</summary>
public interface ICharacterRuleset
{
    /// <summary>Gets the stable uppercase Ruleset identifier.</summary>
    string Id { get; }

    /// <summary>Gets the exact immutable composition contract supported by this module.</summary>
    Type CompositionType { get; }

    /// <summary>Creates structurally valid state before any player decisions.</summary>
    ICharacterComposition Initialize(CharacterContentBinding content);

    /// <summary>Rejects missing, incompatible or structurally invalid state.</summary>
    void Validate(ICharacterComposition composition);
}
