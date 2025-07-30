using System.Diagnostics;

namespace Starlights.Modules.Elements.Domain;

/// <summary>
/// Represents a unique identifier for Element entities.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct ElementId(Guid Value)
{
    /// <summary>
    /// Creates a new ElementId with a Version 7 GUID.
    /// </summary>
    public static ElementId New() => new(Guid.CreateVersion7());

    /// <summary>
    /// Implicitly converts ElementId to Guid.
    /// </summary>
    public static implicit operator Guid(ElementId id) => id.Value;
}