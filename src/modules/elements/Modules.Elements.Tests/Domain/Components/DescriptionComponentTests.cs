using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

/// <summary>
/// Unit tests for <see cref="DescriptionComponent"/>.
/// </summary>
[TestClass]
public class DescriptionComponentTests
{
    [TestMethod]
    public void DescriptionComponent_Constructor_SetsContent()
    {
        // Arrange
        const string expected = "A description.";
        var id = ElementId.New();

        // Act
        var component = new DescriptionComponent(id, expected);

        // Assert
        component.Content.Should().Be(expected);
    }

    [TestMethod]
    public void UpdateContent_UpdatesContent()
    {
        // Arrange
        var component = new DescriptionComponent(ElementId.New(), "Initial");
        const string newContent = "Updated description.";

        // Act
        component.UpdateContent(newContent);

        // Assert
        component.Content.Should().Be(newContent);
    }

    [TestMethod]
    public void Constructor_Throws_WhenContentIsNull()
    {
        // Arrange
        var id = ElementId.New();
        Action actNull = () => _ = new DescriptionComponent(id, null);

        // Assert
        actNull.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Constructor_Allows_EmptyString()
    {
        // Act
        var component = new DescriptionComponent(ElementId.New(), "");

        // Assert
        component.Content.Should().Be("");
    }

    [TestMethod]
    public void UpdateContent_Throws_WhenContentIsNull()
    {
        // Arrange
        var component = new DescriptionComponent(ElementId.New(), "Valid");

        // Act
        Action actNull = () => component.UpdateContent(null!);

        // Assert
        actNull.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void UpdateContent_Allows_EmptyString()
    {
        // Arrange
        var component = new DescriptionComponent(ElementId.New(), "Valid");

        // Act
        component.UpdateContent("");

        // Assert
        component.Content.Should().Be("");
    }
}
