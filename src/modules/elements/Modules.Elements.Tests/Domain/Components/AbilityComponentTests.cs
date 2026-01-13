using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components.Ability;
using Starlights.Modules.Elements.Domain.Values;

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
        var component = new AbilityAspects(ElementId.New(), new Abbreviation(abbreviation));

        // Assert
        component.Abbreviation.Value.Should().Be("STR");
    }

    [TestMethod]
    public void Create_ShouldSetAbbreviation_Uppercase_WhenLowercaseProvided()
    {
        // Arrange
        const string abbreviation = "dex";

        // Act
        var component = new AbilityAspects(ElementId.New(), new Abbreviation(abbreviation));

        // Assert
        component.Abbreviation.Value.Should().Be("DEX");
    }

    [TestMethod]
    public void Create_ShouldSetAbbreviation_Uppercase_WhenMixedCaseProvided()
    {
        // Arrange
        const string abbreviation = "iNt";

        // Act
        var component = new AbilityAspects(ElementId.New(), new Abbreviation(abbreviation));

        // Assert
        component.Abbreviation.Value.Should().Be("INT");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldUpdateAbbreviation_TrimmedAndUppercase_WhenValidAbbreviationProvided()
    {
        // Arrange
        var component = new AbilityAspects(ElementId.New(), new Abbreviation("DEX"));

        // Act
        component.UpdateAbbreviation(new Abbreviation(" Int "));

        // Assert
        component.Abbreviation.Value.Should().Be("INT");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldUpdateAbbreviation_Uppercase_WhenLowercaseProvided()
    {
        // Arrange
        var component = new AbilityAspects(ElementId.New(), new Abbreviation("DEX"));

        // Act
        component.UpdateAbbreviation(new Abbreviation("str"));

        // Assert
        component.Abbreviation.Value.Should().Be("STR");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldUpdateAbbreviation_Uppercase_WhenMixedCaseProvided()
    {
        // Arrange
        var component = new AbilityAspects(ElementId.New(), new Abbreviation("DEX"));

        // Act
        component.UpdateAbbreviation(new Abbreviation("wIs"));

        // Assert
        component.Abbreviation.Value.Should().Be("WIS");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldUpdateAbbreviation_Uppercase_WhenWhitespaceAndMixedCaseProvided()
    {
        // Arrange
        var component = new AbilityAspects(ElementId.New(), new Abbreviation("DEX"));

        // Act
        component.UpdateAbbreviation(new Abbreviation("  cHa  "));

        // Assert
        component.Abbreviation.Value.Should().Be("CHA");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldThrowArgumentException_WhenAbbreviationIsNull()
    {
        // Arrange
        var component = new AbilityAspects(ElementId.New(), new Abbreviation("DEX"));

        // Act
        var act = () => component.UpdateAbbreviation(new Abbreviation(null!));

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldThrowArgumentException_WhenAbbreviationIsWhitespace()
    {
        // Arrange
        var component = new AbilityAspects(ElementId.New(), new Abbreviation("DEX"));

        // Act
        var act = () => component.UpdateAbbreviation(new Abbreviation("   "));

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
