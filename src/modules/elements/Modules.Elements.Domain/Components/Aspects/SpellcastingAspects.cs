using System.Diagnostics;

namespace Starlights.Modules.Elements.Domain.Components.Aspects;

/// <summary>
/// Represents a component that defines aspects of a spell element.
/// </summary>
public sealed class SpellcastingAspects : ElementComponentBase
{
    public SpellcastingAspects(
        ElementId owningElement,
        SpellClassification classification,
        CastingTime castingTime,
        SpellcastingRange range,
        Duration duration)
        : base(owningElement)
    {
        Classification = classification;
        CastingTime = castingTime;
        Range = range;
        Duration = duration;
    }

    /// <summary>
    /// Gets the spell classification details.
    /// </summary>
    public SpellClassification Classification { get; private set; }

    /// <summary>
    /// Gets the spell's casting time.
    /// </summary>
    public CastingTime CastingTime { get; private set; }

    /// <summary>
    /// Gets the spell's range.
    /// </summary>
    public SpellcastingRange Range { get; private set; }

    /// <summary>
    /// Gets the spell's duration.
    /// </summary>
    public Duration Duration { get; private set; }

    /// <summary>
    /// Gets the spell components.
    /// </summary>
    public SpellComponents Components { get; private set; } = new();

    /// <summary>
    /// Updates the spell level.
    /// </summary>
    /// <param name="level">The new level value.</param>
    public void UpdateLevel(int level)
    {
        Classification = Classification with { Level = level };
    }

    /// <summary>
    /// Updates the spell's school of magic.
    /// </summary>
    public void UpdateMagicSchool(string magicSchool)
    {
        Classification = Classification with { MagicSchool = magicSchool };
    }

    /// <summary>
    /// Updates the spell's casting time.
    /// </summary>
    public void UpdateCastingTime(string castingTime)
    {
        CastingTime = CastingTime with { Value = castingTime };
    }

    /// <summary>
    /// Updates the spell's range.
    /// </summary>
    public void UpdateRange(string range)
    {
        Range = Range with { Value = range };
    }

    /// <summary>
    /// Updates the spell's duration.
    /// </summary>
    public void UpdateDuration(Duration duration)
    {
        Duration = duration;
    }

    /// <summary>
    /// Updates whether the spell requires concentration.
    /// </summary>
    public void UpdateIsConcentrationRequired(bool isConcentrationRequired)
    {
        Duration = Duration with { IsConcentration = isConcentrationRequired };
    }

    /// <summary>
    /// Updates whether the spell can be cast as a ritual.
    /// </summary>
    public void UpdateIsRitual(bool isRitual)
    {
        CastingTime = CastingTime with { IsRitual = isRitual };
    }

    /// <summary>
    /// Updates whether the spell has a somatic component.
    /// </summary>
    public void UpdateHasSomaticComponent(bool hasSomaticComponent)
    {
        Components = Components with { HasSomatic = hasSomaticComponent };
    }

    /// <summary>
    /// Updates whether the spell has a verbal component.
    /// </summary>
    public void UpdateHasVerbalComponent(bool hasVerbalComponent)
    {
        Components = Components with { HasVerbal = hasVerbalComponent };
    }

    /// <summary>
    /// Updates the material component flag and description.
    /// </summary>
    /// <param name="hasMaterialComponent">Whether the spell requires a material component.</param>
    /// <param name="materialComponent">The required material component text, if any.</param>
    public void UpdateMaterialComponent(bool hasMaterialComponent, string? materialComponent)
    {
        Components = Components with
        {
            HasMaterial = hasMaterialComponent,
            MaterialComponent = materialComponent
        };
    }
}

[DebuggerDisplay("Level = {Level}, School = {MagicSchool}")]
public record SpellClassification
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

[DebuggerDisplay("Time = {Value}, Ritual = {IsRitual}")]
public record CastingTime
{
    public CastingTime(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        Value = value;
    }

    /// <summary>
    /// Gets the casting time value.
    /// </summary>
    public string Value { get; init; }

    /// <summary>
    /// Indicates whether the spell can be cast as a ritual.
    /// </summary>
    public bool IsRitual { get; init; }
}

[DebuggerDisplay("Range = {Value}")]
public record SpellcastingRange
{
    public SpellcastingRange(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        Value = value;
    }

    /// <summary>
    /// Gets the spellcasting range value.
    /// </summary>
    public string Value { get; init; }

}

[DebuggerDisplay("Duration = {Value}")]
public record Duration
{
    public static Duration Instantaneous => new("Instantaneous");

    public static Duration Concentration(string maxDuration)
    {
        return new Duration(maxDuration, true);
    }

    public Duration(string value, bool isConcentration = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        Value = value;
        IsConcentration = isConcentration;
    }

    /// <summary>
    /// Gets the duration value.
    /// </summary>
    public string Value { get; init; }

    /// <summary>
    /// Indicates whether the duration requires concentration.
    /// </summary>
    public bool IsConcentration { get; init; }
}

[DebuggerDisplay("V = {HasSomatic}, S = {HasVerbal}, M = {HasMaterial}, Material = {MaterialComponent}")]
public record SpellComponents
{
    /// <summary>
    /// Indicates whether the spell has a somatic component.
    /// </summary>
    public bool HasSomatic { get; init; }

    /// <summary>
    /// Indicates whether the spell has a verbal component.
    /// </summary>
    public bool HasVerbal { get; init; }

    /// <summary>
    /// Indicates whether the spell has a material component.
    /// </summary>
    public bool HasMaterial { get; init; }

    /// <summary>
    /// Gets the material component description, if any.
    /// </summary>
    public string? MaterialComponent { get; init => field = value?.Trim(); }
}
