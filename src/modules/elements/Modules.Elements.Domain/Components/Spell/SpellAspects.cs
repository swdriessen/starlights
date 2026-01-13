namespace Starlights.Modules.Elements.Domain.Components.Spell;

/// <summary>
/// Represents a component that defines aspects of a spell element.
/// </summary>
public sealed class SpellAspects : ElementComponentBase
{
    public SpellAspects(ElementId owningElement, SpellClassification classification, CastingTime castingTime, Range range, Duration duration)
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
    public Range Range { get; private set; }

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
        Range = Range with { Type = range };
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
