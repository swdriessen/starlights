namespace Starlights.Modules.Characters.Domain.Building;

/// <summary>Focused Ruleset policies for the supported build decisions; policies never mutate the aggregate.</summary>
public interface ICharacterBuildRules : ICharacterRuleset
{
    /// <summary>Gets the Ruleset supported by this policy.</summary>
    string RulesetId { get; }
    /// <summary>Gets the exact content supported by this policy.</summary>
    CharacterContentBinding Content { get; }
    /// <summary>Validates and records a Selection answer.</summary>
    ICharacterComposition RecordChoice(ICharacterComposition composition, string selectionId, IReadOnlyList<string> answers);
    /// <summary>Validates and assigns ability inputs.</summary>
    ICharacterComposition AssignAbilities(ICharacterComposition composition, IReadOnlyDictionary<string, int> inputs);
    /// <summary>Acquires a selected starting package independently of its Choice.</summary>
    ICharacterComposition ClaimEquipment(ICharacterComposition composition, string selectionId);
    /// <summary>Records armor and weapon allocations to owned item identities.</summary>
    ICharacterComposition Equip(ICharacterComposition composition, string? armorId, string? weaponId);
}
