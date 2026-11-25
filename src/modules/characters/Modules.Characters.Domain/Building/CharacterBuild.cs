namespace Starlights.Modules.Characters.Domain.Building;

/// <summary>Validated immutable initialization state, distinct from a completed build or sheet.</summary>
public sealed class CharacterBuild
{
    /// <summary>Validates state against the selected Ruleset before making it available.</summary>
    public CharacterBuild(ICharacterRuleset ruleset, CharacterContentBinding content, ICharacterComposition composition)
    {
        ruleset.Validate(composition);
        RulesetId = ruleset.Id;
        Content = content;
        Composition = composition;
    }

    /// <summary>Gets the canonical, creation-time Ruleset.</summary>
    public string RulesetId { get; }
    /// <summary>Gets the exact content binding.</summary>
    public CharacterContentBinding Content { get; }
    /// <summary>Gets Ruleset-owned immutable state.</summary>
    public ICharacterComposition Composition { get; }
}
