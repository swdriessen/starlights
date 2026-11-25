using Starlights.Modules.Characters.Domain.Building;

namespace Starlights.RuleSets.DND5E5;

/// <summary>D&D state before a class, origin or other player decision is recorded.</summary>
public sealed record DndCharacterComposition : ICharacterComposition
{
    /// <summary>Creates a composition with exactly one required build part.</summary>
    public DndCharacterComposition(DndBuildPart build)
    {
        if (build is null)
            throw new ArgumentException("The DND5E5 build part is required.", nameof(build));
        Build = build;
    }

    /// <summary>Gets the single immutable build part.</summary>
    public DndBuildPart Build { get; }
}
