using FluentAssertions;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Tests;

[TestClass]
public sealed class RegistrationIncludeRuleTests
{
    [TestMethod]
    public void CreateIncludeRule_ReturnsRuleWithExpectedProperties()
    {
        // Arrange
        var characterId = CharacterId.New();
        var elementId = new ElementId(Guid.NewGuid());
        var registration = Registration.Create(characterId, elementId, "Base Element");

        var includeRuleId = new ElementComponentId(Guid.NewGuid());
        var includedElementId = new ElementId(Guid.NewGuid());
        const string includedElementName = "Included Element";

        // Act
        var rule = registration.CreateIncludeRule(includeRuleId, includedElementId, includedElementName);

        // Assert
        rule.Id.Value.Should().NotBeEmpty();
        rule.ParentRegistrationId.Should().Be(registration.Id);
        rule.AssociatedIncludeRuleId.Should().Be(includeRuleId);
        rule.IncludedElementId.Should().Be(includedElementId);
        rule.IncludedElementName.Should().Be(includedElementName);
    }

    [TestMethod]
    public void CreateIncludeRule_AddsRuleToRegistration()
    {
        // Arrange
        var registration = Registration.Create(CharacterId.New(), new ElementId(Guid.NewGuid()), "Elem");
        var includeRuleId = new ElementComponentId(Guid.NewGuid());

        // Act
        var rule = registration.CreateIncludeRule(includeRuleId, new ElementId(Guid.NewGuid()), "Child");

        // Assert
        registration.IncludeRules.Should().HaveCount(1);
        registration.IncludeRules.Should().ContainSingle(r => r.Id == rule.Id);
        registration.HasAssociatedRule(includeRuleId).Should().BeTrue();
    }

    [TestMethod]
    public void MultipleIncludeRules_HaveDistinctIds()
    {
        // Arrange
        var registration = Registration.Create(CharacterId.New(), new ElementId(Guid.NewGuid()), "Elem");

        // Act
        var first = registration.CreateIncludeRule(new ElementComponentId(Guid.NewGuid()), new ElementId(Guid.NewGuid()), "A");
        var second = registration.CreateIncludeRule(new ElementComponentId(Guid.NewGuid()), new ElementId(Guid.NewGuid()), "B");

        // Assert
        first.Id.Should().NotBe(second.Id);
        registration.IncludeRules.Should().HaveCount(2);
    }
}
