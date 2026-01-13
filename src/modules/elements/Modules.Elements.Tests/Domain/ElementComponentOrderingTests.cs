using AwesomeAssertions;
using Starlights.Modules.Elements.Data;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain;

[TestClass]
public class ElementComponentOrderingTests
{
    [TestMethod]
    public void GetComponents_ShouldReturnIncludeRules_InAddedOrder()
    {
        // Arrange
        var element = Element.Create("Test Element", "TestType");
        var include1 = new IncludeRuleComponent(element.Id, ElementId.New(), levelRequirement: 1);
        var include2 = new IncludeRuleComponent(element.Id, ElementId.New(), levelRequirement: 2);
        var include3 = new IncludeRuleComponent(element.Id, ElementId.New(), levelRequirement: 3);

        element.AddComponent(include1);
        element.AddComponent(include2);
        element.AddComponent(include3);

        // Act
        var ordered = element.GetComponents<IncludeRuleComponent>().ToList();

        // Assert
        ordered.Should().ContainInOrder(include1, include2, include3);
        ordered.Select(c => c.OrderSequence).Should().ContainInOrder(0, 1, 2);
    }

    [TestMethod]
    public void Components_Property_ShouldBeOrderedByOrderSequence()
    {
        // Arrange
        var element = Element.Create("Test Element", "TestType");
        var include1 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 1));
        var include2 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 2));
        var include3 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 3));

        // Act
        var components = element.Components.OfType<IncludeRuleComponent>().ToList();

        // Assert
        components.Should().ContainInOrder(include1, include2, include3);
    }

    [TestMethod]
    public void AsElementDataModel_ShouldPreserveIncludeRuleOrder()
    {
        // Arrange
        var element = Element.Create("Test Element", "TestType");
        var include1 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 1));
        var include2 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 2));
        var include3 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 3));

        // Act
        var model = element.AsElementDataModel();

        // Assert
        model.IncludeRules.Select(r => r.RuleId)
            .Should()
            .ContainInOrder(include1.Id, include2.Id, include3.Id);
    }

    [TestMethod]
    public void MoveComponent_ShouldUpdateOrderSequence_AndOrder()
    {
        // Arrange
        var element = Element.Create("Test Element", "TestType");
        var include1 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 1));
        var include2 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 2));
        var include3 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 3));

        // Act
        element.MoveComponent(include3.Id, 1); // move third to the middle

        // Assert
        var ordered = element.GetComponents<IncludeRuleComponent>().ToList();
        ordered.Should().ContainInOrder(include1, include3, include2);
        ordered.Select(c => c.OrderSequence).Should().ContainInOrder(0, 1, 2);
    }

    [TestMethod]
    public void RemoveComponent_ShouldRemoveComponent_AndUpdateOrderSequence_AndOrder()
    {
        // Arrange
        var element = Element.Create("Test Element", "TestType");
        var include1 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 1));
        var include2 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 2));
        var include3 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 3));

        // Act
        var removed = element.RemoveComponent<IncludeRuleComponent>(include2.Id);

        // Assert
        removed.Should().BeTrue();

        var ordered = element.GetComponents<IncludeRuleComponent>().ToList();
        ordered.Should().ContainInOrder(include1, include3);
        ordered.Select(c => c.OrderSequence).Should().ContainInOrder(0, 1);

        element.Components.OfType<IncludeRuleComponent>()
            .Should()
            .ContainInOrder(include1, include3);

        var model = element.AsElementDataModel();
        model.IncludeRules.Select(r => r.RuleId)
            .Should()
            .ContainInOrder(include1.Id, include3.Id);
    }

    [TestMethod]
    public void RemoveComponent_WhenComponentDoesNotExist_ShouldReturnFalse_AndPreserveOrder()
    {
        // Arrange
        var element = Element.Create("Test Element", "TestType");
        var include1 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 1));
        var include2 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 2));
        var include3 = element.AddComponent(new IncludeRuleComponent(element.Id, ElementId.New(), 3));

        // Act
        var removed = element.RemoveComponent<IncludeRuleComponent>(ElementComponentId.New());

        // Assert
        removed.Should().BeFalse();

        var ordered = element.GetComponents<IncludeRuleComponent>().ToList();
        ordered.Should().ContainInOrder(include1, include2, include3);
        ordered.Select(c => c.OrderSequence).Should().ContainInOrder(0, 1, 2);
    }
}
