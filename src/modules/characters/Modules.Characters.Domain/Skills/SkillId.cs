using System.Diagnostics;

namespace Starlights.Modules.Characters.Domain.Skills;

/// <summary>
/// Represents a unique identifier for Skill entities.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct SkillId(Guid Value)
{
    /// <summary>
    /// Creates a new SkillId with a Version 7 GUID.
    /// </summary>
    public static SkillId New() => new(Guid.CreateVersion7());

    /// <summary>
    /// Implicitly converts SkillId to Guid.
    /// </summary>
    public static implicit operator Guid(SkillId id) => id.Value;
}
