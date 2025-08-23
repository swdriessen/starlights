using System.Diagnostics;

namespace Starlights.Modules.Characters.Domain.Registrations;

/// <summary>
/// Represents a unique identifier for RegistrationSelectionRule entities.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct RegistrationSelectionRuleId(Guid Value)
{
    /// <summary>
    /// Creates a new RegistrationSelectionRuleId with a Version 7 GUID.
    /// </summary>
    public static RegistrationSelectionRuleId New() => new(Guid.CreateVersion7());

    /// <summary>
    /// Implicitly converts RegistrationSelectionRuleId to Guid.
    /// </summary>
    public static implicit operator Guid(RegistrationSelectionRuleId id)
    {
        return id.Value;
    }
}
