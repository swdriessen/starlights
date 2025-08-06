using System.Diagnostics;

namespace Starlights.Modules.Characters.Domain.Registrations;

/// <summary>
/// Represents a unique identifier for Registration entities.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct RegistrationId(Guid Value)
{
    /// <summary>
    /// Creates a new RegistrationId with a Version 7 GUID.
    /// </summary>
    public static RegistrationId New() => new(Guid.CreateVersion7());

    /// <summary>
    /// Implicitly converts RegistrationId to Guid.
    /// </summary>
    public static implicit operator Guid(RegistrationId id) => id.Value;
}
