using System.Diagnostics;

namespace Starlights.Modules.Characters.Domain;

/// <summary>
/// Represents a unique identifier for Appearance entities.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct AppearanceId(Guid Value)
{
    /// <summary>
    /// Creates a new AppearanceId with a Version 7 GUID.
    /// </summary>
    public static AppearanceId New() => new(Guid.CreateVersion7());

    /// <summary>
    /// Implicitly converts AppearanceId to Guid.
    /// </summary>
    public static implicit operator Guid(AppearanceId id) => id.Value;
}
