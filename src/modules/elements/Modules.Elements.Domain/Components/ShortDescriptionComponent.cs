namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Represents a component that holds a short description for an element. The content is limited to a single paragraph (max 500 characters).
/// </summary>
public sealed class ShortDescriptionComponent : ElementComponentBase
{

    /// <summary>
    /// Initializes a new instance of the <see cref="ShortDescriptionComponent"/> class.
    /// </summary>
    /// <param name="owningElement">The owning element's unique identifier.</param>
    /// <param name="content">The short description content.</param>
    public ShortDescriptionComponent(ElementId owningElement, string content)
        : base(owningElement)
    {
        UpdateContent(content);
    }

    /// <summary>
    /// Gets the short description content.
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Updates the short description content.
    /// </summary>
    /// <param name="content">The new short description content.</param>
    public void UpdateContent(string content)
    {
        if (content is null)
        {
            throw new ArgumentException("Short description content cannot be null.", nameof(content));
        }
        Content = content;
    }
}
