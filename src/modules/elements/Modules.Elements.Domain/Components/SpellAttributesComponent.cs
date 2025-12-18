namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Represents a component that defines spell details for an element.
/// </summary>
public sealed class SpellAttributesComponent : ElementComponentBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpellAttributesComponent"/> class.
    /// </summary>
    public SpellAttributesComponent(ElementId owningElement, int level, string magicSchool, string castingTime, string range, string duration)
        : base(owningElement)
    {
        Level = level;
        MagicSchool = magicSchool;
        CastingTime = castingTime;
        Range = range;
        Duration = duration;
    }

    /// <summary>
    /// Gets the spell level.
    /// </summary>
    public int Level { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the spell is a cantrip (level 0).
    /// </summary>
    public bool IsCantrip => Level == 0;

    /// <summary>
    /// Gets the spell's school of magic.
    /// </summary>
    public string MagicSchool { get; private set; }

    /// <summary>
    /// Gets the spell's casting time.
    /// </summary>
    public string CastingTime { get; private set; }

    /// <summary>
    /// Gets the spell's range.
    /// </summary>
    public string Range { get; private set; }

    /// <summary>
    /// Gets the spell's duration.
    /// </summary>
    public string Duration { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the spell requires concentration.
    /// </summary>
    public bool IsConcentrationRequired { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the spell can be cast as a ritual.
    /// </summary>
    public bool IsRitual { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the spell has a somatic component.
    /// </summary>
    public bool HasSomaticComponent { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the spell has a verbal component.
    /// </summary>
    public bool HasVerbalComponent { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the spell has a material component.
    /// </summary>
    public bool HasMaterialComponent { get; private set; }

    /// <summary>
    /// Gets the spell's material component, if any.
    /// </summary>
    public string? MaterialComponent { get; private set; }

    /// <summary>
    /// Updates the spell level.
    /// </summary>
    /// <param name="level">The new level value.</param>
    public void UpdateLevel(int level)
    {
        if (level < 0)
        {
            throw new ArgumentException("Level cannot be negative.", nameof(level));
        }

        Level = level;
    }

    /// <summary>
    /// Updates the spell's school of magic.
    /// </summary>
    public void UpdateMagicSchool(string magicSchool)
    {
        if (string.IsNullOrWhiteSpace(magicSchool))
        {
            throw new ArgumentException("Magic school cannot be null or empty.", nameof(magicSchool));
        }

        MagicSchool = magicSchool.Trim();
    }

    /// <summary>
    /// Updates the spell's casting time.
    /// </summary>
    public void UpdateCastingTime(string castingTime)
    {
        if (string.IsNullOrWhiteSpace(castingTime))
        {
            throw new ArgumentException("Casting time cannot be null or empty.", nameof(castingTime));
        }

        CastingTime = castingTime.Trim();
    }

    /// <summary>
    /// Updates the spell's range.
    /// </summary>
    public void UpdateRange(string range)
    {
        if (string.IsNullOrWhiteSpace(range))
        {
            throw new ArgumentException("Range cannot be null or empty.", nameof(range));
        }

        Range = range.Trim();
    }

    /// <summary>
    /// Updates the spell's duration.
    /// </summary>
    public void UpdateDuration(string duration)
    {
        if (string.IsNullOrWhiteSpace(duration))
        {
            throw new ArgumentException("Duration cannot be null or empty.", nameof(duration));
        }

        Duration = duration.Trim();
    }

    /// <summary>
    /// Updates whether the spell requires concentration.
    /// </summary>
    public void UpdateIsConcentrationRequired(bool isConcentrationRequired)
    {
        IsConcentrationRequired = isConcentrationRequired;
    }

    /// <summary>
    /// Updates whether the spell can be cast as a ritual.
    /// </summary>
    public void UpdateIsRitual(bool isRitual)
    {
        IsRitual = isRitual;
    }

    /// <summary>
    /// Updates whether the spell has a somatic component.
    /// </summary>
    public void UpdateHasSomaticComponent(bool hasSomaticComponent)
    {
        HasSomaticComponent = hasSomaticComponent;
    }

    /// <summary>
    /// Updates whether the spell has a verbal component.
    /// </summary>
    public void UpdateHasVerbalComponent(bool hasVerbalComponent)
    {
        HasVerbalComponent = hasVerbalComponent;
    }

    /// <summary>
    /// Updates the material component flags and text.
    /// </summary>
    /// <param name="hasMaterialComponent">Whether the spell requires a material component.</param>
    /// <param name="materialComponent">The required material component text, if any.</param>
    public void UpdateMaterialComponent(bool hasMaterialComponent, string? materialComponent)
    {
        HasMaterialComponent = hasMaterialComponent;

        if (!hasMaterialComponent)
        {
            MaterialComponent = null;
            return;
        }

        MaterialComponent = string.IsNullOrWhiteSpace(materialComponent) ? null : materialComponent.Trim();
    }
}
