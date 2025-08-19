namespace Starlights.Modules.Elements.Domain.Builders;

public class ElementBuilder
{
    private readonly List<Func<ElementId, ElementComponentBase>> _componentFactories = [];
    private readonly string _type;
    private string? _name;

    private ElementBuilder(string type, string? name = null)
    {
        _type = type;
        _name = name;
    }

    /// <summary>
    /// Sets the name of the element being built.
    /// </summary>
    public ElementBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Adds a component to the element using a factory that receives the element's ID.
    /// </summary>
    public ElementBuilder WithComponent(Func<ElementId, ElementComponentBase> componentFactory)
    {
        _componentFactories.Add(componentFactory);
        return this;
    }

    /// <summary>
    /// Builds the element with the specified name, type, and components.
    /// </summary>
    public Element Build()
    {
        if (string.IsNullOrWhiteSpace(_name))
        {
            throw new InvalidOperationException("Element name must be specified via WithName before calling Build().");
        }

        var element = Element.Create(_name!, _type);

        foreach (var factory in _componentFactories)
        {
            var component = factory(element.Id);
            element.AddComponent(component);
        }

        return element;
    }

    /// <summary>
    /// Creates a new ElementBuilder instance for the given element type.
    /// </summary>
    public static ElementBuilder Create(string type, string? name = null) => new(type, name);
}
