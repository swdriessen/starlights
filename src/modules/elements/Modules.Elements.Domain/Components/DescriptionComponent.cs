namespace Starlights.Modules.Elements.Domain.Components;

/// <summary>
/// Represents a component that holds a description for an element.
/// </summary>
public sealed class DescriptionComponent : ElementComponentBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DescriptionComponent"/> class.
    /// </summary>
    public DescriptionComponent(ElementId owningElement, string content = "")
        : base(owningElement)
    {
        ArgumentNullException.ThrowIfNull(content);
        Content = content.Trim();
    }

    /// <summary>
    /// Gets the description content.
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// Updates the description content.
    /// </summary>
    public void UpdateContent(string content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));
        Content = content.Trim();
    }
}
