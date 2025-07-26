using Starlights.Platform.Domain;

namespace Starlights.Modules.Elements.Domain;

/// <summary>
/// Represents a base class for components of an element.
/// </summary>
public abstract class ElementComponentBase : EntityBase<Guid>
{
    protected ElementComponentBase()
        : base(Guid.NewGuid())
    {
    }
}
