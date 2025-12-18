using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

[TestClass]
public class SpellAttributesComponentTests
{
    [TestMethod]
    public void SpellAttributesComponent_Constructor_SetsLevel()
    {
        // Arrange
        const int expectedLevel = 3;
        const string expectedSchool = "Evocation";

        // Act
        var component = new SpellAttributesComponent(ElementId.New(), expectedLevel, expectedSchool, "1 action", "30 feet", "Instantaneous");

        // Assert
        component.Level.Should().Be(expectedLevel);
        component.MagicSchool.Should().Be(expectedSchool);
    }
}