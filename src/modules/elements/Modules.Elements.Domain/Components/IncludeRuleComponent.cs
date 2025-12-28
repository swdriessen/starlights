namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Represents a component that defines an include rule for an element.
/// </summary>
public sealed class IncludeRuleComponent : ElementComponentBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncludeRuleComponent"/> class.
    /// </summary>
    /// <param name="owningElement">The element this component belongs to.</param>
    /// <param name="includeElement">The element to include.</param>
    /// <param name="levelRequirement">The level requirement for this include rule.</param>
    public IncludeRuleComponent(ElementId owningElement, ElementId includeElement, int levelRequirement)
        : base(owningElement)
    {
        UpdateIncludeElement(includeElement);
        UpdateLevelRequirement(levelRequirement);
    }

    /// <summary>
    /// Gets the element to include.
    /// </summary>
    public ElementId IncludeElement { get; private set; }

    /// <summary>
    /// Gets the display name for this include rule, if any. This is can be used for human readability (imported legacy content may put the old ID_ here).
    /// </summary>
    public string? DisplayName { get; private set; }

    /// <summary>
    /// Gets the level requirement for this include rule.
    /// </summary>
    public int LevelRequirement { get; private set; }

    /// <summary>
    /// Gets or sets a dynamic string that can be used to specify additional requirements for the selection option.
    /// </summary>
    public string? Requirements { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this selection option has any requirements.
    /// </summary>
    public bool HasRequirements => LevelRequirement > 0 || !string.IsNullOrWhiteSpace(Requirements);

    /// <summary>
    /// Updates the element to include.
    /// </summary>
    /// <param name="includeElement">The new element to include.</param>
    public void UpdateIncludeElement(ElementId includeElement)
    {
        if (includeElement == default)
        {
            throw new ArgumentException("IncludeElement cannot be default.", nameof(includeElement));
        }
        IncludeElement = includeElement;
    }

    /// <summary>
    /// Updates the level requirement for this include rule.
    /// </summary>
    /// <param name="levelRequirement">The new level requirement.</param>
    public void UpdateLevelRequirement(int levelRequirement)
    {
        if (levelRequirement < 0)
        {
            throw new ArgumentException("LevelRequirement cannot be negative.", nameof(levelRequirement));
        }
        LevelRequirement = levelRequirement;
    }

    /// <summary>
    /// Updates the display name for this include rule.
    /// </summary>
    /// <param name="displayName">The new display name. Use <see langword="null"/> to clear.</param>
    public void UpdateDisplayName(string? displayName)
    {
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
    }

    /// <summary>
    /// Updates the requirements expression for this include rule.
    /// </summary>
    /// <param name="requirements">The requirements expression. Use <see langword="null"/> to clear.</param>
    public void UpdateRequirements(string? requirements)
    {
        Requirements = string.IsNullOrWhiteSpace(requirements) ? null : requirements.Trim();
    }
}
