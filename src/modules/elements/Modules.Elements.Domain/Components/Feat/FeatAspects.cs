namespace Starlights.Modules.Elements.Domain.Components.Feat;

public class FeatAspects : ElementComponentBase
{
    public FeatAspects(ElementId owningElement, FeatCategory category)
        : base(owningElement)
    {
        Category = category;
    }

    /// <summary>
    /// Gets the category of the feat.
    /// </summary>
    public FeatCategory Category { get; private set; }

    /// <summary>
    /// Updates the category of the feat.
    /// </summary>
    public void UpdateCategory(FeatCategory category)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category.Name);

        if (category == Category)
        {
            return;
        }

        Category = category;
    }
}
