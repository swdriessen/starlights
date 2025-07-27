using FluentAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain;

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
        var element = Element.Create(elementName, elementType);

        // Assert
        element.Name.Should().Be(elementName);
        element.Type.Should().Be(elementType);
        element.Components.Should().BeEmpty();
    }

    [TestMethod]
    public void AddComponent()
    {
        // Arrange
        var element = Element.Create("Strength", ElementTypeConstants.Ability);
        var component = new AbilityComponent(Guid.CreateVersion7(), "STR");

        // Act
        element.AddComponent(component);

        // Assert
        element.Components.OfType<AbilityComponent>().Should().ContainSingle();
    }
}
