using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Values;

namespace Starlights.Modules.Elements.Tests.Domain;

[TestClass]
public sealed class ElementTests
{
    [TestMethod]
    public void CreateShouldHaveDetails()
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
        var component = new AbilityComponent(element.Id, new Abbreviation("STR"));

        // Act
        element.AddComponent(component);

        // Assert
        element.Components.OfType<AbilityComponent>()
            .Should().ContainSingle();
    }

    [TestMethod]
    public void AddComponent_ShouldHaveOwningElement()
    {
        // Arrange
        var element = Element.Create("Strength", ElementTypeConstants.Ability);
        var component = new AbilityComponent(element.Id, new Abbreviation("STR"));

        // Act
        element.AddComponent(component);

        // Assert
        component.OwningElement.Should().Be(element.Id);
    }

    [TestMethod]
    public void RemoveComponent_WhenComponentExists_ShouldRemoveIt()
    {
        // Arrange
        var element = Element.Create("Strength", ElementTypeConstants.Ability);
        var component = element.AddComponent(new AbilityComponent(element.Id, new Abbreviation("STR")));

        // Act
        var removed = element.RemoveComponent<AbilityComponent>(component.Id);

        // Assert
        removed.Should().BeTrue();
        element.Components.OfType<AbilityComponent>().Should().BeEmpty();
    }

    [TestMethod]
    public void RemoveComponent_WhenComponentDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var element = Element.Create("Strength", ElementTypeConstants.Ability);
        element.AddComponent(new AbilityComponent(element.Id, new Abbreviation("STR")));

        // Act
        var removed = element.RemoveComponent<AbilityComponent>(ElementComponentId.New());

        // Assert
        removed.Should().BeFalse();
    }

    [TestMethod]
    public void RemoveComponent_WhenRemovingMiddleComponent_ShouldRenumberOrderSequences()
    {
        // Arrange
        var element = Element.Create("Test Element", "TestType");
        var include1 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 1));
        var include2 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 2));
        var include3 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 3));

        // Act
        element.RemoveComponent<IncludeRuleComponent>(include2.Id);

        // Assert
        var ordered = element.GetComponents<IncludeRuleComponent>().ToList();
        ordered.Should().ContainInOrder(include1, include3);
        ordered.Select(c => c.OrderSequence).Should().ContainInOrder(0, 1);
    }
}
