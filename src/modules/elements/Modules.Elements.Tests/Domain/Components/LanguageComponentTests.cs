using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

[TestClass]
public class LanguageComponentTests
{
    [TestMethod]
    public void Create_ShouldSetOrigin_WhenValidStringProvided()
    {
        // Arrange
        var owningElement = ElementId.New();
        const string origin = "Ancient Greece";

        // Act
        var component = new LanguageComponent(owningElement, origin);

        // Assert
        component.Origin.Should().Be(origin);
        component.Kind.Should().Be("Standard");
    }

    [TestMethod]
    public void UpdateKind_ShouldUpdate_WhenValidStringProvided()
    {
        // Arrange
        var component = new LanguageComponent(ElementId.New(), "Latin");
        const string newKind = "Rare";

        // Act
        component.UpdateKind(newKind);

        // Assert
        component.Kind.Should().Be(newKind);
    }

    [TestMethod]
    public void UpdateKind_ShouldThrowArgumentException_WhenNullProvided()
    {
        // Arrange
        var component = new LanguageComponent(ElementId.New(), "Latin");

        // Act
        var act = () => component.UpdateKind(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void UpdateOrigin_ShouldUpdate_WhenValidStringProvided()
    {
        // Arrange
        var component = new LanguageComponent(ElementId.New(), "Old Norse");
        const string newOrigin = "Modern Icelandic";

        // Act
        component.UpdateOrigin(newOrigin);

        // Assert
        component.Origin.Should().Be(newOrigin);
    }

    [TestMethod]
    public void UpdateOrigin_ShouldAllowEmptyString()
    {
        // Arrange
        var component = new LanguageComponent(ElementId.New(), "Latin");

        // Act
        component.UpdateOrigin("");

        // Assert
        component.Origin.Should().Be("");
    }

    [TestMethod]
    public void UpdateOrigin_ShouldThrowArgumentException_WhenNullProvided()
    {
        // Arrange
        var component = new LanguageComponent(ElementId.New(), "English");

        // Act
        var act = () => component.UpdateOrigin(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
