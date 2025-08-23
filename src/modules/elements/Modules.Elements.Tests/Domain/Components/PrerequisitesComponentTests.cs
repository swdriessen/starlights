using FluentAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

[TestClass]
public class PrerequisitesComponentTests
{
    private static ElementId CreateElementId() => new(Guid.NewGuid());

    [TestMethod]
    public void Constructor_ValidPrerequisites_SetsProperty()
    {
        // Arrange
        var elementId = CreateElementId();
        var prerequisites = "Must be level 10";

        // Act
        var component = new PrerequisitesComponent(elementId, prerequisites);

        // Assert
        component.Prerequisites.Should().Be(prerequisites);
        component.OwningElement.Should().Be(elementId);
    }

    [TestMethod]
    public void UpdatePrerequisites_ValidValue_UpdatesProperty()
    {
        // Arrange
        var elementId = CreateElementId();
        var component = new PrerequisitesComponent(elementId, "Initial");
        var newValue = "Updated prerequisites";

        // Act
        component.UpdatePrerequisites(newValue);

        // Assert
        component.Prerequisites.Should().Be(newValue);
    }
}
