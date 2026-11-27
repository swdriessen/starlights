namespace Starlights.Modules.Characters.Domain.Building;

/// <summary>Ruleset-owned generation of retained ability results, separate from assigning inputs.</summary>
public interface ICharacterAbilityGenerationRules : ICharacterBuildRules
{
    /// <summary>Changes the method, clearing assignments, or explicitly replaces generated results.</summary>
    ICharacterComposition ChangeAbilityGeneration(ICharacterComposition composition, string method, bool reroll);
}
