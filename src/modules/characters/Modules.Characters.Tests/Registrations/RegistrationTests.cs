using FluentAssertions;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Tests.Registrations;

[TestClass]
public sealed class RegistrationTests
{
    [TestMethod]
    public void Registration_Create_SetsProperties()
    {
        // Arrange
        var characterId = CharacterId.New();
        var elementId = new ElementId(Guid.NewGuid());
        const string elementName = "Test Element";
        const string elementType = "Test Type";

        // Act
        var registration = Registration.Create(characterId, elementId, elementName, elementType);

        // Assert
        registration.Id.Value.Should().NotBeEmpty();
        registration.CharacterId.Should().Be(characterId);
        registration.AssociatedElementId.Should().Be(elementId);
        registration.AssociatedElementName.Should().Be(elementName);
        registration.AssociatedElementType.Should().Be(elementType);
        registration.IsProcessed.Should().BeFalse();
        registration.ParentRegistrationId.Should().BeNull();
        registration.IncludeRules.Should().BeEmpty();
    }

    [TestMethod]
    public void Registration_UpdateParentRegistration_SetsParentRegistrationId()
    {
        // Arrange
        var characterId = CharacterId.New();
        var parent = Registration.Create(characterId, new ElementId(Guid.NewGuid()), "Parent", "Type");
        var child = Registration.Create(characterId, new ElementId(Guid.NewGuid()), "Child", "Type");

        // Act
        child.UpdateParentRegistration(parent);

        // Assert
        child.ParentRegistrationId.Should().Be(parent.Id);
    }

    [TestMethod]
    public void Registration_Processed_SetsIsProcessedTrue()
    {
        // Arrange
        var registration = Registration.Create(CharacterId.New(), new ElementId(Guid.NewGuid()), "Elem", "Type");

        // Act
        registration.Processed();

        // Assert
        registration.IsProcessed.Should().BeTrue();
    }

    [TestMethod]
    public void Registration_CreateIncludeRule_AddsRule_And_HasAssociatedRule_IsTrue()
    {
        // Arrange
        var registration = Registration.Create(CharacterId.New(), new ElementId(Guid.NewGuid()), "Elem", "Type");
        var includeRuleId = new ElementComponentId(Guid.NewGuid());
        var includedElementId = new ElementId(Guid.NewGuid());
        const string includedElementName = "Included Element";

        // Act
        var rule = registration.CreateIncludeRule(includeRuleId, includedElementId, includedElementName);

        // Assert
        registration.IncludeRules.Should().HaveCount(1);
        registration.IncludeRules.Should().Contain(rule);
        registration.HasAssociatedRule(includeRuleId).Should().BeTrue();
    }

    [TestMethod]
    public void Registration_HasAssociatedRule_ReturnsFalse_WhenRuleNotPresent()
    {
        // Arrange
        var registration = Registration.Create(CharacterId.New(), new ElementId(Guid.NewGuid()), "Elem", "Type");
        // Add a different rule
        _ = registration.CreateIncludeRule(new ElementComponentId(Guid.NewGuid()), new ElementId(Guid.NewGuid()), "Other");

        // Act
        var result = registration.HasAssociatedRule(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }
}