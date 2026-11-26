using Starlights.Modules.Characters.Domain.Building;

namespace Starlights.RuleSets.DND5E5;

/// <summary>Owns the minimal DND5E5 initialization contract independently of HTTP and persistence.</summary>
public sealed class DndRuleset : ICharacterRuleset
{
    /// <inheritdoc />
    public string Id => "DND5E5";
    /// <inheritdoc />
    public Type CompositionType => typeof(DndCharacterComposition);
    /// <inheritdoc />
    public ICharacterComposition Initialize(CharacterContentBinding content)
        => new DndCharacterComposition(new DndBuildPart("awaiting-choices"),
            content.PackId == "srd-5.2.1" && content.PackVersion == "0.2.0" ? DndBuildFacts.Empty : null);
    /// <inheritdoc />
    public void Validate(ICharacterComposition composition)
    {
        if (composition is not DndCharacterComposition { Build.Stage: "awaiting-choices" })
            throw new InvalidOperationException("DND5E5 requires exactly one supported D&D build part.");
    }
}
