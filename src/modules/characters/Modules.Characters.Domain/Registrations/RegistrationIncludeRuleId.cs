using System.Diagnostics;

namespace Starlights.Modules.Characters.Domain.Registrations;

/// <summary>
/// Represents a unique identifier for RegistrationIncludeRule entities.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct RegistrationIncludeRuleId(Guid Value)
{
    /// <summary>
    /// Creates a new RegistrationIncludeRuleId with a Version 7 GUID.
    /// </summary>
    public static RegistrationIncludeRuleId New() => new(Guid.CreateVersion7());

    /// <summary>
    /// Implicitly converts RegistrationIncludeRuleId to Guid.
    /// </summary>
    public static implicit operator Guid(RegistrationIncludeRuleId id) => id.Value;
}
