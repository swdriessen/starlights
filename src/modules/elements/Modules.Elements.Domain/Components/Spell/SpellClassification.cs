using System.Diagnostics;

namespace Starlights.Modules.Elements.Domain.Components.Spell;

[DebuggerDisplay("Level = {Level}, School = {MagicSchool}")]
public readonly record struct SpellClassification
{
    public SpellClassification(string magicSchool, int level = 0)
    {
        if (level < 0)
        {
            throw new ArgumentException("Level cannot be negative.", nameof(level));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(magicSchool, nameof(magicSchool));

        Level = level;
        MagicSchool = magicSchool.Trim();
    }

    /// <summary>
    /// Gets the spell level.
    /// </summary>
    public int Level { get; init; }

    /// <summary>
    /// Gets the school of magic for the spell.
    /// </summary>
    public string MagicSchool { get; init; }
}
