using FluentAssertions;
using Starlights.Modules.Elements.Domain;

namespace Starlights.Modules.Elements.Tests;

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
        var element = new Element("Strength", "Ability");
        var component = new AbilityComponent("STR");

        // Act
        element.AddComponent(component);

        // Assert
        element.Components.OfType<AbilityComponent>().Should().ContainSingle();
    }
}
