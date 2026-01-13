using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

[TestClass]
public class SortingComponentTests
{
    [TestMethod]
    public void SortingComponent_DefaultConstructor_SetsSortingOrderToZero()
    {
        // Arrange & Act
        var component = new SortingComponent(ElementId.New());

        // Assert
        component.SortingOrder.Should().Be(0);
    }

    [TestMethod]
    public void SortingComponent_Constructor_SetsSortingOrder()
    {
        // Arrange
        const double expected = 42.5;

        // Act
        var component = new SortingComponent(ElementId.New(), expected);

        // Assert
        component.SortingOrder.Should().Be(expected);
    }

    [TestMethod]
    public void UpdateSortingOrder_UpdatesSortingOrder()
    {
        // Arrange
        var component = new SortingComponent(ElementId.New());
        const double newOrder = 99.9;

        // Act
        component.UpdateSortingOrder(newOrder);

        // Assert
        component.SortingOrder.Should().Be(newOrder);
    }
}
