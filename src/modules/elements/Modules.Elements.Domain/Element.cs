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
    public IReadOnlyList<ElementComponentBase> Components => _components.OrderBy(c => c.OrderSequence).ToList().AsReadOnly();

    /// <summary>
    /// Gets the name of the element.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the type of the element.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the identifier of the game system this element belongs to.
    /// </summary>
    public string SystemIdentifier { get; }

    /// <summary>
    /// Updates the name of the element.
    /// </summary>
    /// <param name="newName"></param>
    public void UpdateName(string newName)
    {
        newName = newName.Trim();

        if (Name == newName)
        {
            return;
        }

        Name = newName;
    }

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
        if (component.OwningElement != Id)
        {
            throw new InvalidOperationException("Component's owning element does not match.");
        }
        component.OrderSequence = _components.Count; // append
        _components.Add(component);
        return component;
    }

    /// <summary>
    /// Adds a component to the element using a factory function.
    /// </summary>
    public T AddComponent<T>(Func<ElementId, T> componentFactory) where T : ElementComponentBase
    {
        ArgumentNullException.ThrowIfNull(componentFactory, nameof(componentFactory));
        var component = componentFactory(Id);
        component.OrderSequence = _components.Count; // append
        _components.Add(component);
        return component;
    }

    /// <summary>
    /// Updates a component of the specified type using the provided action.
    /// </summary>
    public Element UpdateComponent<T>(Action<T> updateAction) where T : ElementComponentBase
    {
        ArgumentNullException.ThrowIfNull(updateAction, nameof(updateAction));
        var component = GetRequiredComponent<T>();
        updateAction(component);
        return this;
    }

    /// <summary>
    /// Inserts or moves a component to a specific index, and re-number sequentially starting at 0.
    /// </summary>
    public void MoveComponent(ElementComponentId id, int newIndex)
    {
        if (newIndex < 0 || newIndex >= _components.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(newIndex));
        }

        var idx = _components.FindIndex(c => c.Id == id);
        if (idx < 0)
        {
            throw new InvalidOperationException($"Component with id {id} not found.");
        }

        var item = _components[idx];
        _components.RemoveAt(idx);
        _components.Insert(newIndex, item);

        // Reassign order sequence
        for (int i = 0; i < _components.Count; i++)
        {
            _components[i].OrderSequence = i;
        }
    }

    /// <summary>
    /// Retrieves a single component of the specified type.
    /// </summary>
    public T GetRequiredComponent<T>()
    {
        return _components.OfType<T>().Single();
    }

    /// <summary>
    /// Retrieves a single component of the specified type, or null if not found.
    /// </summary>
    public T? GetComponent<T>()
    {
        return _components.OfType<T>().SingleOrDefault();
    }

    /// <summary>
    /// Retrieves all components of the specified type in their sequence order.
    /// </summary>
    public IEnumerable<T> GetComponents<T>()
    {
        return _components.OrderBy(c => c.OrderSequence).OfType<T>();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Element"/> class with the specified name and type.
    /// </summary>
    public static Element Create(string name, string type, string systemIdentifier = "DND5E") // hardcoded until experimenting with multiple systems
    {
        var element = new Element(name, type, systemIdentifier);
        element.AddDomainEvent(new ElementCreatedEvent(element.Id, name, type));
        return element;
    }
}
