using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

[TestClass]
public class ShortDescriptionComponentTests
{
    [TestMethod]
    public void Create_ShouldSetContent_WhenValidStringProvided()
    {
        // Arrange
        var owningElement = ElementId.New();
        const string content = "A short summary.";

        // Act
        var component = new ShortDescriptionComponent(owningElement, content);

        // Assert
        component.Content.Should().Be(content);
    }

    [TestMethod]
    public void Create_ShouldThrowArgumentException_WhenNullProvided()
    {
        // Arrange
        var owningElement = ElementId.New();

        // Act
        var act = () => new ShortDescriptionComponent(owningElement, null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    // Removed: MaxLength test, as there is no length restriction

    [TestMethod]
    public void UpdateContent_ShouldUpdate_WhenValidStringProvided()
    {
        // Arrange
        var component = new ShortDescriptionComponent(ElementId.New(), "Initial");
        const string newContent = "Updated short description.";

        // Act
        component.UpdateContent(newContent);

        // Assert
        component.Content.Should().Be(newContent);
    }

    [TestMethod]
    public void UpdateContent_ShouldThrowArgumentException_WhenNullProvided()
    {
        // Arrange
        var component = new ShortDescriptionComponent(ElementId.New(), "Initial");

        // Act
        var act = () => component.UpdateContent(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    // Removed: MaxLength test, as there is no length restriction

    [TestMethod]
    public void UpdateContent_ShouldAllowEmptyString()
    {
        // Arrange
        var component = new ShortDescriptionComponent(ElementId.New(), "Initial");

        // Act
        component.UpdateContent("");

        // Assert
        component.Content.Should().Be("");
    }
}
