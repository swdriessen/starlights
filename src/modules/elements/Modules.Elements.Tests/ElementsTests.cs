using FluentAssertions;

namespace Modules.Elements.Tests;

[TestClass]
public sealed class ElementTests
{
    [TestMethod]
    public void Create()
    {
        // Arrange
        const string elementName = "Test Element";
        const string elementType = "Test Type";

        // Act
        var element = new Element(elementName, elementType);

        // Assert
        element.Name.Should().Be(elementName);
        element.Type.Should().Be(elementType);
        element.Components.Should().BeEmpty();
    }

    [TestMethod]
    public void AddComponent()
    {
        // Arrange
        var element = new Element("Test Element", "Test Type");
        var component = new TestComponent("Test Component");

        // Act
        element.AddComponent(component);

        // Assert
        element.Components.Should().Contain(component);
    }

    private sealed class TestComponent : ElementComponentBase
    {
        public TestComponent(string data)
        {
            Data = data;
        }

        public string Data { get; }
    }

}
