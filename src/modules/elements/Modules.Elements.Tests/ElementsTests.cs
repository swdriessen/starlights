using FluentAssertions;

namespace Modules.Elements.Tests;

[TestClass]
public sealed class ElementsTests
{
    [TestMethod]
    public void Create()
    {
        // Arrange
        var elementName = "TestElement";

        // Act
        var element = new Element(elementName);

        // Assert
        element.Name.Should().Be(elementName);
    }
}
