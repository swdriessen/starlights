using Starlights.Platform.Domain;

namespace Modules.Elements;

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
