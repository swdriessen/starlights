using FluentAssertions;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Tests.Registrations;

[TestClass]
public sealed class RegistrationStatisticRuleTests
{
    [TestMethod]
    public void CreateStatisticRule_ReturnsRuleWithExpectedProperties()
    {
        // Arrange
        var characterId = CharacterId.New();
        var elementId = new ElementId(Guid.NewGuid());
        var registration = Registration.Create(characterId, elementId, "Base Element", "Base Type");

        var statisticRuleId = new ElementComponentId(Guid.NewGuid());
        const string name = "hitpoints";
        const string value = "max";

        // Act
        var rule = registration.CreateStatisticRule(statisticRuleId, name, value);

        // Assert
        rule.Id.Value.Should().NotBeEmpty();
        rule.ParentRegistrationId.Should().Be(registration.Id);
        rule.AssociatedStatisticRuleId.Should().Be(statisticRuleId);
        rule.Name.Should().Be(name);
        rule.Value.Should().Be(value);
    }

    [TestMethod]
    public void CreateStatisticRule_AddsRuleToRegistration()
    {
        // Arrange
        var registration = Registration.Create(CharacterId.New(), new ElementId(Guid.NewGuid()), "Elem", "Type");
        var statisticRuleId = new ElementComponentId(Guid.NewGuid());

        // Act
        var rule = registration.CreateStatisticRule(statisticRuleId, "armorclass", "base");

        // Assert
        registration.StatisticRules.Should().HaveCount(1);
        registration.StatisticRules.Should().ContainSingle(r => r.Id == rule.Id);
    }

    [TestMethod]
    public void MultipleStatisticRules_HaveDistinctIds()
    {
        // Arrange
        var registration = Registration.Create(CharacterId.New(), new ElementId(Guid.NewGuid()), "Elem", "Type");

        // Act
        var first = registration.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "hp", "10");
        var second = registration.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "hp", "20");

        // Assert
        first.Id.Should().NotBe(second.Id);
        registration.StatisticRules.Should().HaveCount(2);
    }
}
