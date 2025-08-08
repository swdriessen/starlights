using System.Diagnostics;

namespace Starlights.Modules.Characters.Domain.Elements;

/// <summary>
/// Represents a unique identifier for associated element entities in the context of the Characters module.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct ElementId(Guid Value)
{
    /// <summary>
    /// Implicitly converts ElementId to Guid.
    /// </summary>
    public static implicit operator Guid(ElementId id) => id.Value;
}
