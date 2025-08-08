using System.Diagnostics;

namespace Starlights.Modules.Characters.Domain.Elements;

/// <summary>
/// Represents a unique identifier for associated element component entities in the context of the Characters module.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct ElementComponentId(Guid Value)
{
    /// <summary>
    /// Implicitly converts ElementComponentId to Guid.
    /// </summary>
    public static implicit operator Guid(ElementComponentId id) => id.Value;
}