using System.Diagnostics;

namespace Starlights.Modules.Characters.Domain.Abilities;

/// <summary>
/// Represents a unique identifier for AbilityScore entities.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct AbilityScoreId(Guid Value)
{
    /// <summary>
    /// Creates a new AbilityScoreId with a Version 7 GUID.
    /// </summary>
    public static AbilityScoreId New() => new(Guid.CreateVersion7());

    /// <summary>
    /// Implicitly converts AbilityScoreId to Guid.
    /// </summary>
    public static implicit operator Guid(AbilityScoreId id) => id.Value;
}