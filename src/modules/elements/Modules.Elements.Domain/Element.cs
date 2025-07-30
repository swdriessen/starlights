using Starlights.Platform.Domain;

namespace Starlights.Modules.Elements.Domain;

/// <summary>
/// Represents an element (building block) in the system, which can have multiple components.
/// </summary>
public sealed class Element : AggregateRoot<ElementId>
{
    private readonly List<ElementComponentBase> _components = [];

    private Element(string name, string type)
        : base(ElementId.New())
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

    /// <summary>
    /// Creates a new instance of the <see cref="Element"/> class with the specified name and type.
    /// </summary>
    public static Element Create(string name, string type)
    {
        var element = new Element(name, type);
        // raise an 'ElementCreated' domain event here if needed
        return element;
    }

    /// <summary>
    /// Retrieves a component of the specified type from the element's components.
    /// </summary>
    public T GetComponent<T>()
    {
        return _components.OfType<T>().Single();
    }

    /// <summary>
    /// Retrieves a component of the specified type from the element's components.
    /// </summary>
    public IEnumerable<T> GetComponents<T>()
    {
        return _components.OfType<T>();
    }
}
