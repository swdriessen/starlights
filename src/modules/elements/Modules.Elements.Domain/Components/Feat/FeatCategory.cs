using System.Diagnostics;

namespace Starlights.Modules.Elements.Domain.Components.Feat;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly record struct FeatCategory
{
    public FeatCategory(ElementId id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name;
    }

    /// <summary>
    /// Gets the ID of the feat category.
    /// </summary>
    public ElementId Id { get; init; }

    /// <summary>
    /// Gets the name of the feat category.
    /// </summary>
    public string Name { get; init; }

    private string GetDebuggerDisplay()
    {
        return $"FeatCategory {{ Id = {Id}, Name = {Name} }}";
    }
}