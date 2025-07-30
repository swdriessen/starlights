using FluentAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

[TestClass]
public class IncludeRuleComponentTests
{
    private static ElementId CreateElementId() => new(Guid.NewGuid());

    [TestMethod]
    public void Constructor_ValidParameters_SetsProperties()
    {
        // Arrange
        var owningElement = CreateElementId();
        var includeElement = CreateElementId();
        const int levelRequirement = 5;

        // Act
        var component = new IncludeRuleComponent(owningElement, includeElement, levelRequirement);

        // Assert
        component.IncludeElement.Should().Be(includeElement);
        component.LevelRequirement.Should().Be(levelRequirement);
        component.OwningElement.Should().Be(owningElement);
    }

    [TestMethod]
    public void Constructor_DefaultIncludeElement_ThrowsArgumentException()
    {
        // Arrange
        var owningElement = CreateElementId();
        var includeElement = default(ElementId);
        const int levelRequirement = 5;

        // Act & Assert
        var action = () => new IncludeRuleComponent(owningElement, includeElement, levelRequirement);
        action.Should().Throw<ArgumentException>()
            .WithMessage("IncludeElement cannot be default. (Parameter 'includeElement')");
    }

    [TestMethod]
    public void Constructor_NegativeLevelRequirement_ThrowsArgumentException()
    {
        // Arrange
        var owningElement = CreateElementId();
        var includeElement = CreateElementId();
        const int levelRequirement = -1;

        // Act & Assert
        var action = () => new IncludeRuleComponent(owningElement, includeElement, levelRequirement);
        action.Should().Throw<ArgumentException>()
            .WithMessage("LevelRequirement cannot be negative. (Parameter 'levelRequirement')");
    }

    [TestMethod]
    public void Constructor_ZeroLevelRequirement_SetsProperty()
    {
        // Arrange
        var owningElement = CreateElementId();
        var includeElement = CreateElementId();
        const int levelRequirement = 0;

        // Act
        var component = new IncludeRuleComponent(owningElement, includeElement, levelRequirement);

        // Assert
        component.LevelRequirement.Should().Be(levelRequirement);
    }

    [TestMethod]
    public void UpdateIncludeElement_ValidElement_UpdatesProperty()
    {
        // Arrange
        var owningElement = CreateElementId();
        var initialElement = CreateElementId();
        var newElement = CreateElementId();
        var component = new IncludeRuleComponent(owningElement, initialElement, 1);

        // Act
        component.UpdateIncludeElement(newElement);

        // Assert
        component.IncludeElement.Should().Be(newElement);
    }

    [TestMethod]
    public void UpdateIncludeElement_DefaultElement_ThrowsArgumentException()
    {
        // Arrange
        var owningElement = CreateElementId();
        var initialElement = CreateElementId();
        var component = new IncludeRuleComponent(owningElement, initialElement, 1);

        // Act & Assert
        var action = () => component.UpdateIncludeElement(default);
        action.Should().Throw<ArgumentException>()
            .WithMessage("IncludeElement cannot be default. (Parameter 'includeElement')");
    }

    [TestMethod]
    public void UpdateLevelRequirement_ValidLevel_UpdatesProperty()
    {
        // Arrange
        var owningElement = CreateElementId();
        var includeElement = CreateElementId();
        var component = new IncludeRuleComponent(owningElement, includeElement, 1);
        const int newLevel = 10;

        // Act
        component.UpdateLevelRequirement(newLevel);

        // Assert
        component.LevelRequirement.Should().Be(newLevel);
    }

    [TestMethod]
    public void UpdateLevelRequirement_ZeroLevel_UpdatesProperty()
    {
        // Arrange
        var owningElement = CreateElementId();
        var includeElement = CreateElementId();
        var component = new IncludeRuleComponent(owningElement, includeElement, 5);
        const int newLevel = 0;

        // Act
        component.UpdateLevelRequirement(newLevel);

        // Assert
        component.LevelRequirement.Should().Be(newLevel);
    }

    [TestMethod]
    public void UpdateLevelRequirement_NegativeLevel_ThrowsArgumentException()
    {
        // Arrange
        var owningElement = CreateElementId();
        var includeElement = CreateElementId();
        var component = new IncludeRuleComponent(owningElement, includeElement, 1);

        // Act & Assert
        var action = () => component.UpdateLevelRequirement(-1);
        action.Should().Throw<ArgumentException>()
            .WithMessage("LevelRequirement cannot be negative. (Parameter 'levelRequirement')");
    }
}
