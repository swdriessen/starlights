using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components.Spell;
using Range = Starlights.Modules.Elements.Domain.Components.Spell.Range;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

[TestClass]
public class SpellAspectsTests
{
    [TestMethod]
    public void SpellAspects_Constructor_SetsClassification()
    {
        // Arrange
        const int expectedLevel = 3;
        const string expectedSchool = "Evocation";
        var classification = new SpellClassification(expectedSchool, expectedLevel);
        var time = new CastingTime("1 action");
        var range = new Range("30 feet");

        var duration = Duration.Instantaneous;

        // Act
        var component = new SpellAspects(ElementId.New(), classification, time, range, duration);

        // Assert
        component.Classification.Level.Should().Be(expectedLevel);
        component.Classification.MagicSchool.Should().Be(expectedSchool);
    }

    [TestMethod]
    public void SpellAspects_Constructor_SetsCastingTime()
    {
        // Arrange
        var time = new CastingTime("1 action");

        // Act
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), time, new Range("30 feet"), Duration.Instantaneous);

        // Assert
        component.CastingTime.Should().Be(time);
    }

    [TestMethod]
    public void SpellAspects_Constructor_SetsRange()
    {
        // Arrange
        var range = new Range("30 feet");

        // Act
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), new CastingTime("1 action"), range, Duration.Instantaneous);

        // Assert
        component.Range.Should().Be(range);
    }

    [TestMethod]
    public void SpellAspects_Constructor_SetsDuration()
    {
        // Arrange
        var duration = Duration.Concentration("1 minute");

        // Act
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), new CastingTime("1 action"), new Range("30 feet"), duration);

        // Assert
        component.Duration.Should().Be(duration);
    }

    [TestMethod]
    public void SpellAspects_Constructor_DefaultsComponentsToEmpty()
    {
        // Act
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), new CastingTime("1 action"), new Range("30 feet"), Duration.Instantaneous);

        // Assert
        component.Components.Should().Be(default(SpellComponents));
    }

    [TestMethod]
    public void SpellAspects_UpdateLevel_UpdatesLevel()
    {
        // Arrange
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), new CastingTime("1 action"), new Range("30 feet"), Duration.Instantaneous);

        // Act
        component.UpdateLevel(5);

        // Assert
        component.Classification.Level.Should().Be(5);
    }

    [TestMethod]
    public void SpellAspects_UpdateMagicSchool_UpdatesMagicSchool()
    {
        // Arrange
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), new CastingTime("1 action"), new Range("30 feet"), Duration.Instantaneous);

        // Act
        component.UpdateMagicSchool("Transmutation");

        // Assert
        component.Classification.MagicSchool.Should().Be("Transmutation");
    }

    [TestMethod]
    public void SpellAspects_UpdateCastingTime_UpdatesCastingTime()
    {
        // Arrange
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), new CastingTime("1 action"), new Range("30 feet"), Duration.Instantaneous);

        // Act
        component.UpdateCastingTime("1 reaction");

        // Assert
        component.CastingTime.Value.Should().Be("1 reaction");
    }

    [TestMethod]
    public void SpellAspects_UpdateRange_UpdatesRange()
    {
        // Arrange
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), new CastingTime("1 action"), new Range("30 feet"), Duration.Instantaneous);

        // Act
        component.UpdateRange("Self");

        // Assert
        component.Range.Type.Should().Be("Self");
    }

    [TestMethod]
    public void SpellAspects_UpdateDuration_UpdatesDuration()
    {
        // Arrange
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), new CastingTime("1 action"), new Range("30 feet"), Duration.Instantaneous);
        var duration = new Duration("10 minutes");

        // Act
        component.UpdateDuration(duration);

        // Assert
        component.Duration.Should().Be(duration);
    }

    [TestMethod]
    public void SpellAspects_UpdateIsConcentrationRequired_UpdatesDurationIsConcentration()
    {
        // Arrange
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), new CastingTime("1 action"), new Range("30 feet"), new Duration("1 minute"));

        // Act
        component.UpdateIsConcentrationRequired(true);

        // Assert
        component.Duration.IsConcentration.Should().BeTrue();
    }

    [TestMethod]
    public void SpellAspects_UpdateIsRitual_UpdatesCastingTimeIsRitual()
    {
        // Arrange
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), new CastingTime("1 action"), new Range("30 feet"), Duration.Instantaneous);

        // Act
        component.UpdateIsRitual(true);

        // Assert
        component.CastingTime.IsRitual.Should().BeTrue();
    }

    [TestMethod]
    public void SpellAspects_UpdateHasSomaticComponent_UpdatesHasSomaticComponent()
    {
        // Arrange
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), new CastingTime("1 action"), new Range("30 feet"), Duration.Instantaneous);

        // Act
        component.UpdateHasSomaticComponent(true);

        // Assert
        component.Components.HasSomatic.Should().BeTrue();
    }

    [TestMethod]
    public void SpellAspects_UpdateHasVerbalComponent_UpdatesHasVerbalComponent()
    {
        // Arrange
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), new CastingTime("1 action"), new Range("30 feet"), Duration.Instantaneous);

        // Act
        component.UpdateHasVerbalComponent(true);

        // Assert
        component.Components.HasVerbal.Should().BeTrue();
    }

    [TestMethod]
    public void SpellAspects_UpdateMaterialComponent_UpdatesMaterialComponent()
    {
        // Arrange
        var component = new SpellAspects(ElementId.New(), new SpellClassification("Evocation", 3), new CastingTime("1 action"), new Range("30 feet"), Duration.Instantaneous);

        // Act
        component.UpdateMaterialComponent(true, " a bit of fleece ");

        // Assert
        component.Components.HasMaterial.Should().BeTrue();
        component.Components.MaterialComponent.Should().Be("a bit of fleece");
    }
}