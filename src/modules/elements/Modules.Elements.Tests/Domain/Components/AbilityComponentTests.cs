using FluentAssertions;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

[TestClass]
public class AbilityComponentTests
{
    [TestMethod]
    public void Create_ShouldSetAbbreviation_TrimmedAndUppercase_WhenValidAbbreviationProvided()
    {
        // Arrange
        const string abbreviation = " Str ";

        // Act
        var component = new AbilityComponent(Guid.CreateVersion7(), abbreviation);

        // Assert
        component.Abbreviation.Should().Be("STR");
    }

    [TestMethod]
    public void Create_ShouldSetAbbreviation_Uppercase_WhenLowercaseProvided()
    {
        // Arrange
        const string abbreviation = "dex";

        // Act
        var component = new AbilityComponent(Guid.CreateVersion7(), abbreviation);

        // Assert
        component.Abbreviation.Should().Be("DEX");
    }

    [TestMethod]
    public void Create_ShouldSetAbbreviation_Uppercase_WhenMixedCaseProvided()
    {
        // Arrange
        const string abbreviation = "iNt";

        // Act
        var component = new AbilityComponent(Guid.CreateVersion7(), abbreviation);

        // Assert
        component.Abbreviation.Should().Be("INT");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldUpdateAbbreviation_TrimmedAndUppercase_WhenValidAbbreviationProvided()
    {
        // Arrange
        var component = new AbilityComponent(Guid.CreateVersion7(), "DEX");

        // Act
        component.UpdateAbbreviation(" Int ");

        // Assert
        component.Abbreviation.Should().Be("INT");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldUpdateAbbreviation_Uppercase_WhenLowercaseProvided()
    {
        // Arrange
        var component = new AbilityComponent(Guid.CreateVersion7(), "DEX");

        // Act
        component.UpdateAbbreviation("str");

        // Assert
        component.Abbreviation.Should().Be("STR");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldUpdateAbbreviation_Uppercase_WhenMixedCaseProvided()
    {
        // Arrange
        var component = new AbilityComponent(Guid.CreateVersion7(), "DEX");

        // Act
        component.UpdateAbbreviation("wIs");

        // Assert
        component.Abbreviation.Should().Be("WIS");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldUpdateAbbreviation_Uppercase_WhenWhitespaceAndMixedCaseProvided()
    {
        // Arrange
        var component = new AbilityComponent(Guid.CreateVersion7(), "DEX");

        // Act
        component.UpdateAbbreviation("  cHa  ");

        // Assert
        component.Abbreviation.Should().Be("CHA");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldThrowArgumentException_WhenAbbreviationIsNull()
    {
        // Arrange
        var component = new AbilityComponent(Guid.CreateVersion7(), "DEX");

        // Act
        var act = () => component.UpdateAbbreviation(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldThrowArgumentException_WhenAbbreviationIsWhitespace()
    {
        // Arrange
        var component = new AbilityComponent(Guid.CreateVersion7(), "DEX");

        // Act
        var act = () => component.UpdateAbbreviation("   ");

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
