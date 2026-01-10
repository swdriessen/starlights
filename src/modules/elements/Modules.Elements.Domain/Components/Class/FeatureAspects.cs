namespace Starlights.Modules.Elements.Domain.Components.Class;

public sealed class FeatureAspects : ElementComponentBase
{
    public FeatureAspects(ElementId owningElement, int level, double listingOrder = 0)
        : base(owningElement)
    {
        Level = level;
        ListingOrder = listingOrder;
    }

    /// <summary>
    /// Gets the level at which the feature is acquired. This is optionally displayed in the name of the feature.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// Gets the order in which the feature is listed among other features of the same class and level.
    /// </summary>
    public double ListingOrder { get; }
}
