using FluentAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

[TestClass]
public class PrimaryAbilityComponentTests
{
    [TestMethod]
    public void Create_ShouldSetPrimaryAbility_WhenValidElementIdProvided()
    {
        // Arrange
        var owningElement = ElementId.New();
        var primaryAbility = ElementId.New();

        // Act
        var component = new PrimaryAbilityComponent(owningElement, primaryAbility);

        // Assert
        component.PrimaryAbility.Should().Be(primaryAbility);
        component.OwningElement.Should().Be(owningElement);
    }

    [TestMethod]
    public void UpdatePrimaryAbility_ShouldUpdate_WhenValidElementIdProvided()
    {
        // Arrange
        var owningElement = ElementId.New();
        var initialAbility = ElementId.New();
        var newAbility = ElementId.New();
        var component = new PrimaryAbilityComponent(owningElement, initialAbility);

        // Act
        component.UpdatePrimaryAbility(newAbility);

        // Assert
        component.PrimaryAbility.Should().Be(newAbility);
    }

    [TestMethod]
    public void UpdatePrimaryAbility_ShouldThrowArgumentException_WhenDefaultProvided()
    {
        // Arrange
        var owningElement = ElementId.New();
        var initialAbility = ElementId.New();
        var component = new PrimaryAbilityComponent(owningElement, initialAbility);

        // Act
        var act = () => component.UpdatePrimaryAbility(default);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
