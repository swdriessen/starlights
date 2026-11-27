using Starlights.Modules.Characters.Domain.Building;
using Starlights.RuleSets.DND5E5.Building;

namespace Starlights.RuleSets.DND5E5.Rules;

/// <summary>
/// Owns the minimal DND5E5 initialization contract independently of HTTP and persistence.
/// </summary>
public sealed class Ruleset : ICharacterRuleset
{

    /// <inheritdoc />
    public string Id => "DND5E5";

    /// <inheritdoc />
    public Type CompositionType => typeof(CharacterComposition);

    /// <inheritdoc />
    public ICharacterComposition Initialize(CharacterContentBinding content)
    {
        return new CharacterComposition(new BuildPart("awaiting-choices"),
                content.PackId == "srd-5.2.1" && content.PackVersion == "0.2.0" ? BuildFacts.Empty : null);
    }

    /// <inheritdoc />
    public void Validate(ICharacterComposition composition)
    {
        if (composition is not CharacterComposition { Build.Stage: "awaiting-choices" })
        {
            throw new InvalidOperationException("DND5E5 requires exactly one supported D&D build part.");
        }
    }
}
