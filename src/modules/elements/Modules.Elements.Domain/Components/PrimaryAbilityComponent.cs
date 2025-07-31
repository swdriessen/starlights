namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Represents a component that identifies the primary ability of an element.
/// </summary>
public sealed class PrimaryAbilityComponent : ElementComponentBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrimaryAbilityComponent"/> class.
    /// </summary>
    /// <param name="owningElement">The element this component belongs to.</param>
    /// <param name="primaryAbility">The primary ability identifier.</param>
    public PrimaryAbilityComponent(ElementId owningElement, ElementId primaryAbility)
        : base(owningElement)
    {
        PrimaryAbility = primaryAbility;
    }

    /// <summary>
    /// Gets the primary ability identifier.
    /// </summary>
    public ElementId PrimaryAbility { get; private set; }

    /// <summary>
    /// Updates the primary ability identifier.
    /// </summary>
    /// <param name="primaryAbility">The new primary ability identifier.</param>
    public void UpdatePrimaryAbility(ElementId primaryAbility)
    {
        if (primaryAbility == default)
        {
            throw new ArgumentException("PrimaryAbility cannot be default.", nameof(primaryAbility));
        }
        PrimaryAbility = primaryAbility;
    }
}
