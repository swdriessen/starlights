using FluentAssertions;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Tests;

[TestClass]
public sealed class RegistrationSelectionRuleTests
{
    [TestMethod]
    public void CreateSelectionRule_ReturnsRuleWithExpectedProperties()
    {
        // Arrange
        var characterId = CharacterId.New();
        var elementId = new ElementId(Guid.NewGuid());
        var registration = Registration.Create(characterId, elementId, "Base Element", "Base Type");

        var selectionRuleId = new ElementComponentId(Guid.NewGuid());
        const string elementType = "Skill";
        const string name = "Choose a Skill";

        // Act
        var rule = registration.CreateSelectionRule(selectionRuleId, elementType, name);

        // Assert
        rule.Id.Value.Should().NotBeEmpty();
        rule.ParentRegistrationId.Should().Be(registration.Id);
        rule.AssociatedSelectionRuleId.Should().Be(selectionRuleId);
        rule.ElementType.Should().Be(elementType);
        rule.Name.Should().Be(name);
    }

    [TestMethod]
    public void CreateSelectionRule_AddsRuleToRegistration()
    {
        // Arrange
        var registration = Registration.Create(CharacterId.New(), new ElementId(Guid.NewGuid()), "Elem", "Type");
        var selectionRuleId = new ElementComponentId(Guid.NewGuid());

        // Act
        var rule = registration.CreateSelectionRule(selectionRuleId, "Feat", "Pick a Feat");

        // Assert
        registration.SelectionRules.Should().HaveCount(1);
        registration.SelectionRules.Should().ContainSingle(r => r.Id == rule.Id);
    }

    [TestMethod]
    public void MultipleSelectionRules_HaveDistinctIds()
    {
        // Arrange
        var registration = Registration.Create(CharacterId.New(), new ElementId(Guid.NewGuid()), "Elem", "Type");

        // Act
        var first = registration.CreateSelectionRule(new ElementComponentId(Guid.NewGuid()), "Feat", "Pick a Feat");
        var second = registration.CreateSelectionRule(new ElementComponentId(Guid.NewGuid()), "Feat", "Pick another Feat");

        // Assert
        first.Id.Should().NotBe(second.Id);
        registration.SelectionRules.Should().HaveCount(2);
    }
}
