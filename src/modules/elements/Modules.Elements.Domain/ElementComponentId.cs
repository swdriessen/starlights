using System.Diagnostics;

namespace Starlights.Modules.Elements.Domain;

/// <summary>
/// Represents a unique identifier for ElementComponent entities.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct ElementComponentId(Guid Value)
{
    /// <summary>
    /// Creates a new ElementComponentId with a Version 7 GUID.
    /// </summary>
    public static ElementComponentId New() => new(Guid.CreateVersion7());

    /// <summary>
    /// Implicitly converts ElementComponentId to Guid.
    /// </summary>
    public static implicit operator Guid(ElementComponentId id) => id.Value;
}