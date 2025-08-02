using System.Diagnostics;

namespace Starlights.Modules.Characters.Domain;

/// <summary>
/// Represents a unique identifier for Character entities.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct CharacterId(Guid Value)
{
    /// <summary>
    /// Creates a new CharacterId with a Version 7 GUID.
    /// </summary>
    public static CharacterId New() => new(Guid.CreateVersion7());

    /// <summary>
    /// Implicitly converts CharacterId to Guid.
    /// </summary>
    public static implicit operator Guid(CharacterId id) => id.Value;
}