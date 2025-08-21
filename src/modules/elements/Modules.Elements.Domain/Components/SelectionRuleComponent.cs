namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Represents a component that defines a selection rule for an element.
/// </summary>
public sealed class SelectionRuleComponent : ElementComponentBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionRuleComponent"/> class.
    /// </summary>
    public SelectionRuleComponent(ElementId owningElement, string elementType, string name, int levelRequirement)
        : base(owningElement)
    {
        ElementType = elementType.Trim();
        Name = name.Trim();
        UpdateLevelRequirement(levelRequirement);
    }

    /// <summary>
    /// Gets the element type for this selection rule.
    /// </summary>
    public string ElementType { get; }

    /// <summary>
    /// Gets the descriptive name of the selection option.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a short description for the selection option(s).
    /// </summary>
    public string? ShortDescription { get; private set; }

    /// <summary>
    /// Gets the supports string of the selection option.
    /// </summary>
    public string? Supports { get; private set; }

    /// <summary>
    /// Gets the range supports string of the selection option. The elements in the range will be included in the selection options.
    /// </summary>
    public string? RangeSupports { get; private set; }

    /// <summary>
    /// Gets a dynamic string that can be used to specify additional requirements for the selection option.
    /// </summary>
    public string? Requirements { get; private set; }

    /// <summary>
    /// Gets the level requirement for this selection rule.
    /// </summary>
    public int LevelRequirement { get; private set; }

    /// <summary>
    /// Gets the quantity of elements that can be selected by this rule.
    /// </summary>
    public int Quantity { get; private set; } = 1;

    /// <summary>
    /// Gets a value indicating whether this selection option is optional.
    /// </summary>
    public bool IsOptional { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this selection option allows multiple selections.
    /// </summary>
    public bool IsMultiSelection => Quantity > 1;

    /// <summary>
    /// Gets a value indicating whether this selection option has any requirements.
    /// </summary>
    public bool HasRequirements => LevelRequirement > 0 || !string.IsNullOrWhiteSpace(Requirements);

    /// <summary>
    /// Gets a value indicating whether this selection option has any supports defined.
    /// </summary>
    public bool HasSupports => !string.IsNullOrWhiteSpace(Supports) || !string.IsNullOrWhiteSpace(RangeSupports);

    /// <summary>
    /// Updates the short description.
    /// </summary>
    public void UpdateShortDescription(string? description) => ShortDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

    /// <summary>
    /// Updates the supports string.
    /// </summary>
    public void UpdateSupports(string? supports) => Supports = string.IsNullOrWhiteSpace(supports) ? null : supports.Trim();

    /// <summary>
    /// Updates the range supports string.
    /// </summary>
    public void UpdateRangeSupports(string? supports) => RangeSupports = string.IsNullOrWhiteSpace(supports) ? null : supports.Trim();

    /// <summary>
    /// Updates the additional requirements string.
    /// </summary>
    public void UpdateRequirements(string? requirements) => Requirements = string.IsNullOrWhiteSpace(requirements) ? null : requirements.Trim();

    /// <summary>
    /// Updates the level requirement.
    /// </summary>
    public void UpdateLevelRequirement(int levelRequirement)
    {
        if (levelRequirement < 0)
        {
            throw new ArgumentException("LevelRequirement cannot be negative.", nameof(levelRequirement));
        }
        LevelRequirement = levelRequirement;
    }

    /// <summary>
    /// Updates the quantity allowed for this selection rule.
    /// </summary>
    public void UpdateQuantity(int quantity)
    {
        if (quantity < 1)
        {
            throw new ArgumentException("Quantity must be at least 1.", nameof(quantity));
        }
        Quantity = quantity;
    }

    /// <summary>
    /// Marks the rule optional or required.
    /// </summary>
    public void UpdateIsOptional(bool isOptional) => IsOptional = isOptional;
}
