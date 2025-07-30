namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Represents a component that holds a description for an element.
/// </summary>
public sealed class DescriptionComponent : ElementComponentBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DescriptionComponent"/> class.
    /// </summary>
    /// <param name="owningElement">The owning element's unique identifier.</param>
    /// <param name="content">The description content.</param>
    public DescriptionComponent(ElementId owningElement, string content)
        : base(owningElement)
    {
        UpdateContent(content);
    }

    /// <summary>
    /// Gets the description content.
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Updates the description content.
    /// </summary>
    /// <param name="content">The new description content.</param>
    public void UpdateContent(string content)
    {
        if (content is null)
        {
            throw new ArgumentException("Description content cannot be null.", nameof(content));
        }
        Content = content;
    }
}
