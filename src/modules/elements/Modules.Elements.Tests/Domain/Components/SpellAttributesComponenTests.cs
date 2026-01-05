using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components.Spellcasting;

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
        var classification = new SpellClassification(expectedSchool, expectedLevel);
        var time = new CastingTime("1 action");
        var range = new Elements.Domain.Components.Spellcasting.Range("30 feet");
        var duration = Duration.Instantaneous;

        // Act
        var component = new SpellcastingAspects(ElementId.New(), classification, time, range, duration);

        // Assert
        component.Classification.Level.Should().Be(expectedLevel);
        component.Classification.MagicSchool.Should().Be(expectedSchool);
    }
}