using System.Diagnostics;

namespace Starlights.Modules.Characters.Domain.Registrations;

/// <summary>
/// Represents a unique identifier for RegistrationStatisticRule entities.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct RegistrationStatisticRuleId(Guid Value)
{
    /// <summary>
    /// Creates a new RegistrationStatisticRuleId with a Version 7 GUID.
    /// </summary>
    public static RegistrationStatisticRuleId New() => new(Guid.CreateVersion7());

    /// <summary>
    /// Implicitly converts RegistrationStatisticRuleId to Guid.
    /// </summary>
    public static implicit operator Guid(RegistrationStatisticRuleId id)
    {
        return id.Value;
    }
}
