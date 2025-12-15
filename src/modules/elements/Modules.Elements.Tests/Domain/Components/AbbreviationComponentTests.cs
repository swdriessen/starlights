using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

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
        var component = new AbbreviationComponent(ElementId.New(), abbreviation);

        // Assert
        component.Abbreviation.Should().Be("XYZ");
    }

    [TestMethod]
    public void Create_ShouldSetAbbreviation_Uppercase_WhenLowercaseProvided()
    {
        // Arrange
        const string abbreviation = "abc";

        // Act
        var component = new AbbreviationComponent(ElementId.New(), abbreviation);

        // Assert
        component.Abbreviation.Should().Be("ABC");
    }

    [TestMethod]
    public void Create_ShouldSetAbbreviation_Uppercase_WhenMixedCaseProvided()
    {
        // Arrange
        const string abbreviation = "aBc";

        // Act
        var component = new AbbreviationComponent(ElementId.New(), abbreviation);

        // Assert
        component.Abbreviation.Should().Be("ABC");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldUpdateAbbreviation_TrimmedAndUppercase_WhenValidAbbreviationProvided()
    {
        // Arrange
        var component = new AbbreviationComponent(ElementId.New(), "DEF");

        // Act
        component.UpdateAbbreviation("  ghi  ");

        // Assert
        component.Abbreviation.Should().Be("GHI");
    }

    [TestMethod]
    public void UpdateAbbreviation_ShouldThrowArgumentException_WhenAbbreviationIsNullOrWhitespace()
    {
        // Arrange
        var component = new AbbreviationComponent(ElementId.New(), "ABC");

        // Act
        var actNull = () => component.UpdateAbbreviation(null!);
        var actWhitespace = () => component.UpdateAbbreviation("   ");

        // Assert
        actNull.Should().Throw<ArgumentException>();
        actWhitespace.Should().Throw<ArgumentException>();
    }
}
