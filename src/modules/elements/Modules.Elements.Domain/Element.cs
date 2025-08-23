using Starlights.Platform.Domain;

namespace Starlights.Modules.Elements.Domain;

/// <summary>
/// Represents an element (building block) in the system, which can have multiple components.
/// </summary>
public sealed class Element : AggregateRoot<ElementId>
{
    private readonly List<ElementComponentBase> _components = [];

    private Element(string name, string type, string systemIdentifier)
        : base(ElementId.New())
    {
        Name = name.Trim();
        Type = type.Trim();
        SystemIdentifier = systemIdentifier.Trim();
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
    /// Gets the identifier of the game system this element belongs to.
    /// </summary>
    public string SystemIdentifier { get; }

    /// <summary>
    /// Sets the ID of the element. This may be used when importing data instead of creating new elements.
    /// </summary>
    /// <remarks>
    /// This should be used with caution as it can affect references to this element.
    /// </remarks>
    public void SetElementId(ElementId newId)
    {
        if (newId == default)
        {
            throw new ArgumentException("New ID cannot be default.", nameof(newId));
        }

        Id = newId;
    }

    /// <summary>
    /// Adds a component to the element.
    /// </summary>
    public T AddComponent<T>(T component) where T : ElementComponentBase
    {
        ArgumentNullException.ThrowIfNull(component, nameof(component));
        _components.Add(component);
        return component;
    }

    /// <summary>
    /// Retrieves a component of the specified type.
    /// </summary>
    public T GetComponent<T>() => _components.OfType<T>().Single();

    /// <summary>
    /// Retrieves all components of the specified type.
    /// </summary>
    public IEnumerable<T> GetComponents<T>() => _components.OfType<T>();

    /// <summary>
    /// Creates a new instance of the <see cref="Element"/> class with the specified name and type.
    /// </summary>
    public static Element Create(string name, string type, string systemIdentifier = "DND5E") // hardcoded until experimenting with multiple systems
    {
        var element = new Element(name, type, systemIdentifier);
        // raise an 'ElementCreated' domain event here if needed
        return element;
    }
}
