using Starlights.Platform.Domain;

namespace Starlights.Modules.Elements.Domain;

/// <summary>
/// Represents an element (building block) in the system, which can have multiple components.
/// </summary>
public class Element : AggregateRoot<Guid>
{
    private readonly List<ElementComponentBase> _components = [];

    public Element(string name, string type)
        : base(Guid.NewGuid())
    {
        Name = name.Trim();
        Type = type.Trim();
    }

    /// <summary>
    /// Gets the components associated with the element.
    /// </summary>
    public IReadOnlyList<ElementComponentBase> Components => _components.AsReadOnly();

    /// <summary>
    /// Gets the name of the element.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type of the element.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Adds a component to the element.
    /// </summary>
    public void AddComponent<T>(T component) where T : ElementComponentBase
    {
        ArgumentNullException.ThrowIfNull(component, nameof(component));
        _components.Add(component);
    }
}
