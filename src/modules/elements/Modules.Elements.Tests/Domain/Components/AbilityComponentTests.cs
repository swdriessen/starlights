using FluentAssertions;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

[TestClass]
public class AbilityComponentTests
{
    [TestMethod]
    public void Create_ShouldSetAbbreviation_WhenValidAbbreviationProvided()
    {
        // Arrange
        const string abbreviation = " STR ";

        // Act
        var component = new AbilityComponent(abbreviation);

        // Assert
        component.Abbreviation.Should().Be("STR");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldUpdateAbbreviation_WhenValidAbbreviationProvided()
    {
        // Arrange
        var component = new AbilityComponent("DEX");

        // Act
        component.UpdateAbbreviation(" INT ");

        // Assert
        component.Abbreviation.Should().Be("INT");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldThrowArgumentException_WhenAbbreviationIsNull()
    {
        // Arrange
        var component = new AbilityComponent("DEX");

        // Act
        var act = () => component.UpdateAbbreviation(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldThrowArgumentException_WhenAbbreviationIsWhitespace()
    {
        // Arrange
        var component = new AbilityComponent("DEX");

        // Act
        var act = () => component.UpdateAbbreviation("   ");

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
