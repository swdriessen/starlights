namespace Starlights.Modules.Elements.Domain.Components;

public class FeatAttributesComponent : ElementComponentBase
{
    public FeatAttributesComponent(ElementId owningElement, ElementId categoryId, string category)
        : base(owningElement)
    {
        CategoryId = categoryId;
        Category = category;
    }

    /// <summary>
    /// Gets the category id.
    /// </summary>
    public ElementId CategoryId { get; private set; }

    /// <summary>
    /// Gets the category of the feat.
    /// </summary>
    public string Category { get; private set; }

    /// <summary>
    /// Updates the category of the feat.
    /// </summary>
    public void UpdateCategory(ElementId categoryId, string category)
    {
        if (categoryId == CategoryId)
        {
            return;
        }

        CategoryId = categoryId;
        Category = category;
    }
}