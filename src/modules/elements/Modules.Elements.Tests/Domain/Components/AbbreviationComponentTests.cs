using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Domain.Values;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

[TestClass]
public class AbbreviationComponentTests
{
    [TestMethod]
    public void Create_ShouldSetAbbreviation_TrimmedAndUppercase_WhenValidAbbreviationProvided()
    {
        // Arrange
        const string abbreviation = "  xyz  ";

        // Act
        var component = new AbbreviationComponent(ElementId.New(), new Abbreviation(abbreviation));

        // Assert
        component.Abbreviation.Value.Should().Be("XYZ");
    }

    [TestMethod]
    public void Create_ShouldSetAbbreviation_Uppercase_WhenLowercaseProvided()
    {
        // Arrange
        const string abbreviation = "abc";

        // Act
        var component = new AbbreviationComponent(ElementId.New(), new Abbreviation(abbreviation));

        // Assert
        component.Abbreviation.Value.Should().Be("ABC");
    }

    [TestMethod]
    public void Create_ShouldSetAbbreviation_Uppercase_WhenMixedCaseProvided()
    {
        // Arrange
        const string abbreviation = "aBc";

        // Act
        var component = new AbbreviationComponent(ElementId.New(), new Abbreviation(abbreviation));

        // Assert
        component.Abbreviation.Value.Should().Be("ABC");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldUpdateAbbreviation_TrimmedAndUppercase_WhenValidAbbreviationProvided()
    {
        // Arrange
        var component = new AbbreviationComponent(ElementId.New(), new Abbreviation("DEF"));

        // Act
        component.UpdateAbbreviation(new Abbreviation("  ghi  "));

        // Assert
        component.Abbreviation.Value.Should().Be("GHI");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldThrowArgumentException_WhenAbbreviationIsNullOrWhitespace()
    {
        // Arrange
        var component = new AbbreviationComponent(ElementId.New(), new Abbreviation("ABC"));

        // Act
        var actNull = () => component.UpdateAbbreviation(new Abbreviation(null!));
        var actWhitespace = () => component.UpdateAbbreviation(new Abbreviation("   "));

        // Assert
        actNull.Should().Throw<ArgumentException>();
        actWhitespace.Should().Throw<ArgumentException>();
    }
}
