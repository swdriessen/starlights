using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

[TestClass]
public class StatisticRuleComponentTests
{
    [TestMethod]
    public void Constructor_ValidParameters_SetsPropertiesNormalized()
    {
        // Arrange
        var owningElement = ElementId.New();
        const string name = "  HitPoints  ";
        const string value = "  MaX  ";
        const int levelRequirement = 3;

        // Act
        var component = new StatisticRuleComponent(owningElement, name, value, levelRequirement);

        // Assert
        component.OwningElement.Should().Be(owningElement);
        component.Name.Should().Be("hitpoints");
        component.Value.Should().Be("max");
        component.LevelRequirement.Should().Be(levelRequirement);
        component.StackingBonus.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_NegativeLevelRequirement_ThrowsArgumentException()
    {
        // Arrange
        var owningElement = ElementId.New();

        // Act
        var act = () => new StatisticRuleComponent(owningElement, "hp", "10", -1);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void UpdateName_ShouldNormalizeToLowercaseAndTrim()
    {
        // Arrange
        var component = new StatisticRuleComponent(ElementId.New(), "hp", "10", 0);

        // Act
        component.UpdateName("  ArMoRClass  ");

        // Assert
        component.Name.Should().Be("armorclass");
    }

    [TestMethod]
    public void UpdateValue_ShouldNormalizeToLowercaseAndTrim()
    {
        // Arrange
        var component = new StatisticRuleComponent(ElementId.New(), "hp", "10", 0);

        // Act
        component.UpdateValue("  MaX ");

        // Assert
        component.Value.Should().Be("max");
    }

    [TestMethod]
    public void UpdateStackingBonus_ShouldNormalizeToLowercaseAndTrim()
    {
        // Arrange
        var component = new StatisticRuleComponent(ElementId.New(), "hp", "10", 0);

        // Act
        component.UpdateStackingBonus("  BoNuS  ");

        // Assert
        component.StackingBonus.Should().Be("bonus");
    }

    [TestMethod]
    public void IsNumberValue_ShouldReturnTrue_WhenValueIsNumeric()
    {
        // Arrange
        var component = new StatisticRuleComponent(ElementId.New(), "hp", "123", 0);

        // Act
        var result = component.IsNumberValue();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void IsNumberValue_ShouldReturnFalse_WhenValueIsNotNumeric()
    {
        // Arrange
        var component = new StatisticRuleComponent(ElementId.New(), "hp", "abc", 0);

        // Act
        var result = component.IsNumberValue();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void GetValue_ShouldReturnInteger_WhenValueIsNumeric()
    {
        // Arrange
        var component = new StatisticRuleComponent(ElementId.New(), "hp", "42", 0);

        // Act
        var value = component.GetValue();

        // Assert
        value.Should().Be(42);
    }

    [TestMethod]
    public void GetValue_ShouldThrowInvalidOperationException_WhenValueIsNotNumeric()
    {
        // Arrange
        var component = new StatisticRuleComponent(ElementId.New(), "hp", "notnumber", 0);

        // Act
        var act = () => component.GetValue();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void UpdateLevelRequirement_ShouldSetValue_WhenNonNegative()
    {
        // Arrange
        var component = new StatisticRuleComponent(ElementId.New(), "hp", "10", 0);

        // Act
        component.UpdateLevelRequirement(7);

        // Assert
        component.LevelRequirement.Should().Be(7);
    }

    [TestMethod]
    public void UpdateLevelRequirement_ShouldThrowArgumentException_WhenNegative()
    {
        // Arrange
        var component = new StatisticRuleComponent(ElementId.New(), "hp", "10", 0);

        // Act
        var act = () => component.UpdateLevelRequirement(-5);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
