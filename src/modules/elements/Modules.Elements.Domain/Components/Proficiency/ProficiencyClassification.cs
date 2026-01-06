using System.Diagnostics;

namespace Starlights.Modules.Elements.Domain.Components.Proficiency;

[DebuggerDisplay("{Type}")]
public sealed record ProficiencyClassification
{
    public static readonly ProficiencyClassification Skill = new("Skill");
    public static readonly ProficiencyClassification SavingThrow = new("Saving Throw");
    public static readonly ProficiencyClassification Weapon = new("Weapon");
    public static readonly ProficiencyClassification Armor = new("Armor");
    public static readonly ProficiencyClassification Tool = new("Tool");

    public ProficiencyClassification(string type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        Type = type.Trim();
    }

    /// <summary>
    /// Gets the type of proficiency classification.
    /// </summary>
    public string Type { get; }

    public static implicit operator string(ProficiencyClassification classification)
    {
        return classification.Type;
    }
}
