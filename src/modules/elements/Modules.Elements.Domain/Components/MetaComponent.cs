namespace Starlights.Modules.Elements.Domain.Components;

public sealed class MetaComponent : ElementComponentBase
{
    public MetaComponent(ElementId owningElement, ParentElement? parent = null)
        : base(owningElement)
    {
        Parent = parent;
    }

    /// <summary>
    /// Gets the parent element for categorization purposes.
    /// </summary>
    public ParentElement? Parent { get; }
}

/// <summary>
/// Represents a parent element with its identifier and name.
/// </summary>
public sealed record ParentElement
{
    public ParentElement(ElementId id, string name)
    {
        Id = id;
        Name = name;
    }

    public ElementId Id { get; }
    public string Name { get; }
}