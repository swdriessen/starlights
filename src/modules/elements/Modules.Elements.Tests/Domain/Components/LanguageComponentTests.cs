using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components.Language;

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
        var component = new LanguageAspects(owningElement, LanguageClassification.Standard, origin);

        // Assert
        component.Origin.Should().Be(origin);
        component.Classification.Should().Be(LanguageClassification.Standard);
    }

    [TestMethod]
    public void UpdateKind_ShouldUpdate_WhenValidStringProvided()
    {
        // Arrange
        var component = new LanguageAspects(ElementId.New(), LanguageClassification.Standard, "Latin");
        var newKind = LanguageClassification.Rare;

        // Act
        component.UpdateClassification(newKind);

        // Assert
        component.Classification.Should().Be(newKind);
    }

    [TestMethod]
    public void UpdateKind_ShouldThrowArgumentException_WhenNullProvided()
    {
        // Arrange
        var component = new LanguageAspects(ElementId.New(), LanguageClassification.Standard, "Latin");

        // Act
        var act = () => component.UpdateClassification(new LanguageClassification(null!));

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void UpdateOrigin_ShouldUpdate_WhenValidStringProvided()
    {
        // Arrange
        var component = new LanguageAspects(ElementId.New(), LanguageClassification.Standard, "Old Norse");
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
        var component = new LanguageAspects(ElementId.New(), LanguageClassification.Standard, "Latin");

        // Act
        component.UpdateOrigin("");

        // Assert
        component.Origin.Should().Be("");
    }

    [TestMethod]
    public void UpdateOrigin_ShouldThrowArgumentException_WhenNullProvided()
    {
        // Arrange
        var component = new LanguageAspects(ElementId.New(), LanguageClassification.Standard, "English");

        // Act
        var act = () => component.UpdateOrigin(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
