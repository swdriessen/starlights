using Starlights.Platform.Domain;

namespace Starlights.Modules.Elements.Domain;

/// <summary>
/// Represents a base class for components of an element.
/// </summary>
public abstract class ElementComponentBase : EntityBase<ElementComponentId>
{
    protected ElementComponentBase(ElementId owningElement)
        : base(ElementComponentId.New())
    {
        OwningElement = owningElement;
    }

    /// <summary>
    /// Gets the unique identifier of the parent element that this component belongs to.
    /// </summary>
    public ElementId OwningElement { get; protected set; }

    /// <summary>
    /// Gets the explicit order sequence of this component within its parent element.
    /// Used to ensure deterministic ordering and support manual reordering.
    /// </summary>
    public int OrderSequence { get; internal set; }
}
