namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Represents a component that defines a sorting order for an element.
/// </summary>
public sealed class SortingComponent : ElementComponentBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortingComponent"/> class.
    /// </summary>
    /// <param name="sortingOrder">The sorting order value.</param>
    public SortingComponent(Guid owningElement, double sortingOrder = 0)
        : base(owningElement)
    {
        SortingOrder = sortingOrder;
    }

    /// <summary>
    /// Gets the sorting order of the component.
    /// </summary>
    public double SortingOrder { get; private set; } = 0;

    /// <summary>
    /// Updates the sorting order of the component.
    /// </summary>
    /// <param name="sortingOrder">The new sorting order value.</param>
    public void UpdateSortingOrder(double sortingOrder)
    {
        SortingOrder = sortingOrder;
    }
}
