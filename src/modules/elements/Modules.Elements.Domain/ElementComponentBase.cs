using Starlights.Platform.Domain;

namespace Starlights.Modules.Elements.Domain;

/// <summary>
/// Represents a base class for components of an element.
/// </summary>
public abstract class ElementComponentBase : EntityBase<Guid>
{
    protected ElementComponentBase(Guid owningElement)
        : base(Guid.CreateVersion7())
    {
        OwningElement = owningElement;
    }

    /// <summary>
    /// Gets the unique identifier of the element (parent) that this component belongs to.
    /// </summary>
    public Guid OwningElement { get; protected set; }
}
