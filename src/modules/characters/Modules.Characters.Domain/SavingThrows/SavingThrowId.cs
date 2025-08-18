using System.Diagnostics;

namespace Starlights.Modules.Characters.Domain.SavingThrows;

/// <summary>
/// Represents a unique identifier for SavingThrow entities.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct SavingThrowId(Guid Value)
{
    /// <summary>
    /// Creates a new SavingThrowId with a Version 7 GUID.
    /// </summary>
    public static SavingThrowId New() => new(Guid.CreateVersion7());

    /// <summary>
    /// Implicitly converts SavingThrowId to Guid.
    /// </summary>
    public static implicit operator Guid(SavingThrowId id) => id.Value;
}
