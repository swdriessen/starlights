using AwesomeAssertions;

namespace Starlights.Integration.Drivers;

public sealed class ElementsDriverContext
{
    private readonly List<DriverElement> _createdElements = [];

    public void WithCreatedElement(Guid id, string name, string type = "UNKNOWN")
    {
        _createdElements.Add(new DriverElement(id, name, type));
    }

    public DriverElement CurrentElement
    {
        get
        {
            _createdElements.Should().NotBeEmpty("No element has been created in this scenario.");
            return _createdElements[^1];
        }
    }

    public DriverElement GetElement(string name)
    {
        var element = _createdElements.SingleOrDefault(e => e.Name == name);
        element.Id.Should().NotBeEmpty("No element with name '{0}' has been created in this scenario.", name);
        return element;
    }

    public DriverElement GetElement(string name, string type)
    {
        var typed = _createdElements.Where(x => x.Type == type);
        typed.Should().ContainSingle(e => e.Name == name, "No element with name '{0}' and type '{1}' has been created in this scenario.", name, type);

        var element = typed.SingleOrDefault(e => e.Name == name);
        element.Id.Should().NotBeEmpty("No element with name '{0}' and type '{1}' has been created in this scenario.", name, type);

        return element;
    }

    public record struct DriverElement(Guid Id, string Name, string Type);
}
