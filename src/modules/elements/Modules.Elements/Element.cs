using Starlights.Platform.Domain;

namespace Modules.Elements;

public class Element : AggregateRoot<Guid>
{
    public Element(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the name of the element.
    /// </summary>
    public string Name { get; }
}
