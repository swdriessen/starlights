using FluentAssertions;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Services;

namespace Starlights.Modules.Characters.Tests.Services;

[TestClass]
public sealed class StatisticsCalculatorTests
{
    private StatisticsCalculator _calculator = default!;

    [TestInitialize]
    public void Initialize()
    {
        _calculator = new StatisticsCalculator();
    }

    [TestMethod]
    public void Calculate_WithNoRegistrations_ShouldReturnEmptyStatistics()
    {
        // Arrange
        var character = Character.Create("Test Character");
        character.AddComponent(ProgressionComponent.Create(character.Id));
        character.AddComponent(AbilitiesComponent.Create(character.Id));
        var registrations = new List<Registration>();

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<StatisticValuesGroupCollection>();
    }

    [TestMethod]
    public void Calculate_WithCharacterLevel_ShouldSeedLevelStatistics()
    {
        // Arrange
        var character = Character.Create("Test Character");
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(5);
        character.AddComponent(progressionComponent);
        character.AddComponent(AbilitiesComponent.Create(character.Id));
        var registrations = new List<Registration>();

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.ContainsGroup("level").Should().BeTrue();
        result.GetValue("level").Should().Be(5);
        result.GetGroup("level").GetValues().Should().ContainSingle(v => v.DisplayName == "Character Level");

        result.ContainsGroup("character:level").Should().BeTrue();
        result.GetValue("character:level").Should().Be(5);
    }

    [TestMethod]
    public void Calculate_WithAbilityScores_ShouldSeedAbilityStatistics()
    {
        // Arrange
        var character = Character.Create("Test Character");
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(1);
        character.AddComponent(progressionComponent);

        var abilitiesComponent = AbilitiesComponent.Create(character.Id);
        var registration = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Strength", "Ability");
        var abilityScore = abilitiesComponent.CreateAbilityScore(registration.Id, "Strength", "STR");
        abilityScore.UpdateBaseScore(16); // Score 16 = modifier +3
        character.AddComponent(abilitiesComponent);
        var registrations = new List<Registration>();

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.ContainsGroup("str:score").Should().BeTrue();
        result.GetValue("str:score").Should().Be(16);
        result.GetGroup("str:score").GetValues().Should().ContainSingle(v => v.DisplayName == "Strength");

        result.ContainsGroup("str:modifier").Should().BeTrue();
        result.GetValue("str:modifier").Should().Be(3);
        result.GetGroup("str:modifier").GetValues().Should().ContainSingle(v => v.DisplayName == "Strength Modifier");

        result.ContainsGroup("str:modifier_half").Should().BeTrue();
        result.GetValue("str:modifier_half").Should().Be(1); // floor(3/2) = 1

        result.ContainsGroup("str:modifier_half_up").Should().BeTrue();
        result.GetValue("str:modifier_half_up").Should().Be(2); // ceil(3/2) = 2
    }

    [TestMethod]
    public void Calculate_WithStatisticRules_ShouldProcessDirectValuesWithDisplayNames()
    {
        // Arrange
        var character = Character.Create("Test Character");
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(3);
        character.AddComponent(progressionComponent);
        character.AddComponent(AbilitiesComponent.Create(character.Id));

        var registration = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Test Feature", "ClassFeature");
        var statisticRule = registration.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "test-stat", "+5");
        statisticRule.UpdateLevelRequirement(1);
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.ContainsGroup("test-stat").Should().BeTrue();
        result.GetValue("test-stat").Should().Be(5);
        result.GetGroup("test-stat").GetValues().Should().ContainSingle()
            .Which.DisplayName.Should().Be("Test Feature");
    }

    [TestMethod]
    public void Calculate_WithLevelRequirements_ShouldFilterInvalidRules()
    {
        // Arrange
        var character = Character.Create("Test Character");
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(2);
        character.AddComponent(progressionComponent);
        character.AddComponent(AbilitiesComponent.Create(character.Id));

        var registration = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Test Feature", "ClassFeature");
        var lowLevelRule = registration.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "test-stat-1", "1");
        lowLevelRule.UpdateLevelRequirement(1);

        var highLevelRule = registration.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "test-stat-2", "2");
        highLevelRule.UpdateLevelRequirement(5);
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.ContainsGroup("test-stat-1").Should().BeTrue();
        result.GetValue("test-stat-1").Should().Be(1);
        result.ContainsGroup("test-stat-2").Should().BeFalse("Level requirement not met");
    }

    [TestMethod]
    public void Calculate_WithStackingBonus_ShouldUseHighestValue()
    {
        // Arrange
        var character = Character.Create("Test Character");
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(5);
        character.AddComponent(progressionComponent);
        character.AddComponent(AbilitiesComponent.Create(character.Id));

        var registration1 = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Feature 1", "ClassFeature");
        var rule1 = registration1.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "armor-class", "+2");
        rule1.UpdateStackingBonus("enhancement");

        var registration2 = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Feature 2", "ClassFeature");
        var rule2 = registration2.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "armor-class", "+3");
        rule2.UpdateStackingBonus("enhancement");
        var registrations = new List<Registration> { registration1, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.ContainsGroup("armor-class").Should().BeTrue();
        result.GetValue("armor-class").Should().Be(3, "Only the highest stacking bonus should be applied");
        result.GetGroup("armor-class").GetValues().Should().ContainSingle()
            .Which.DisplayName.Should().Contain("enhancement");
    }

    [TestMethod]
    public void Calculate_WithReferenceValue_ShouldResolveReferences()
    {
        // Arrange
        var character = Character.Create("Test Character");
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(1);
        character.AddComponent(progressionComponent);

        var abilitiesComponent = AbilitiesComponent.Create(character.Id);
        var abilityRegistration = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Strength", "Ability");
        var abilityScore = abilitiesComponent.CreateAbilityScore(abilityRegistration.Id, "Strength", "STR");
        abilityScore.UpdateBaseScore(14); // Modifier +2
        character.AddComponent(abilitiesComponent);

        var registration = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Test Feature", "ClassFeature");
        var referenceRule = registration.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "attack-bonus", "str:modifier");
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.ContainsGroup("attack-bonus").Should().BeTrue();
        result.GetValue("attack-bonus").Should().Be(2, "Should resolve to the strength modifier value");
        result.GetGroup("attack-bonus").GetValues().Should().ContainSingle()
            .Which.DisplayName.Should().Be("Test Feature");
    }

    [TestMethod]
    public void Calculate_WithMaximumValue_ShouldApplyCap()
    {
        // Arrange
        var character = Character.Create("Test Character");
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(1);
        character.AddComponent(progressionComponent);
        character.AddComponent(AbilitiesComponent.Create(character.Id));

        var registration = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Test Feature", "ClassFeature");
        var rule = registration.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "test-stat", "100");
        rule.UpdateMaximumValue(20);
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.ContainsGroup("test-stat").Should().BeTrue();
        result.GetValue("test-stat").Should().Be(20, "Value should be capped at maximum");
        result.GetGroup("test-stat").GetValues().Should().Contain(v => v.DisplayName!.Contains("Max Cap"));
    }

    [TestMethod]
    public void Calculate_WithMinimumValue_ShouldApplyFloor()
    {
        // Arrange
        var character = Character.Create("Test Character");
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(1);
        character.AddComponent(progressionComponent);
        character.AddComponent(AbilitiesComponent.Create(character.Id));

        var registration = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Test Feature", "ClassFeature");
        var rule = registration.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "test-stat", "1");
        rule.UpdateMinimumValue(5);
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.ContainsGroup("test-stat").Should().BeTrue();
        result.GetValue("test-stat").Should().Be(5, "Value should be raised to minimum");
        result.GetGroup("test-stat").GetValues().Should().Contain(v => v.DisplayName!.Contains("Min Floor"));
    }

    [TestMethod]
    public void Calculate_WithProficiencyStatistic_ShouldCreateVariants()
    {
        // Arrange
        var character = Character.Create("Test Character");
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(5);
        character.AddComponent(progressionComponent);
        character.AddComponent(AbilitiesComponent.Create(character.Id));

        var registration = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Proficiency Rule", "Rule");
        var proficiencyRule = registration.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "proficiency", "3");
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.ContainsGroup("proficiency").Should().BeTrue();
        result.GetValue("proficiency").Should().Be(3);
        result.ContainsGroup("proficiency:bonus").Should().BeTrue();
        result.GetValue("proficiency:bonus").Should().Be(3);
        result.GetGroup("proficiency:bonus").GetValues().Should().ContainSingle(v => v.DisplayName == "Proficiency Bonus");

        result.ContainsGroup("proficiency:half").Should().BeTrue();
        result.GetValue("proficiency:half").Should().Be(1); // floor(3/2) = 1
        result.ContainsGroup("proficiency:half_up").Should().BeTrue();
        result.GetValue("proficiency:half_up").Should().Be(2); // ceil(3/2) = 2
    }

    [TestMethod]
    public void Calculate_WithMultipleAdditiveRules_ShouldSumValues()
    {
        // Arrange
        var character = Character.Create("Test Character");
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(5);
        character.AddComponent(progressionComponent);
        character.AddComponent(AbilitiesComponent.Create(character.Id));

        var registration1 = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Feature 1", "ClassFeature");
        registration1.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "bonus-stat", "+2");

        var registration2 = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Feature 2", "ClassFeature");
        registration2.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "bonus-stat", "+3");
        var registrations = new List<Registration> { registration1, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.ContainsGroup("bonus-stat").Should().BeTrue();
        result.GetValue("bonus-stat").Should().Be(5, "Multiple additive bonuses should sum");
        result.GetGroup("bonus-stat").GetValues().Should().HaveCount(2);
        result.GetGroup("bonus-stat").GetValues().Should().Contain(v => v.DisplayName == "Feature 1");
        result.GetGroup("bonus-stat").GetValues().Should().Contain(v => v.DisplayName == "Feature 2");
    }

    [TestMethod]
    public void Calculate_AllGroupsShouldBeFinalized()
    {
        // Arrange
        var character = Character.Create("Test Character");
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(1);
        character.AddComponent(progressionComponent);
        character.AddComponent(AbilitiesComponent.Create(character.Id));
        var registrations = new List<Registration>();

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().OnlyContain(group => group.IsFinalized);
    }

    [TestMethod]
    public void Calculate_StatisticValues_ShouldIncludeRuleIds()
    {
        // Arrange
        var character = Character.Create("Test Character");
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(1);
        character.AddComponent(progressionComponent);
        character.AddComponent(AbilitiesComponent.Create(character.Id));

        var registration = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Test Feature", "ClassFeature");
        var ruleId = new ElementComponentId(Guid.NewGuid());
        registration.CreateStatisticRule(ruleId, "test-stat", "+5");
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.GetGroup("test-stat").GetValues().Should().ContainSingle()
            .Which.RuleId.Should().Be(ruleId.Value);
    }

    [TestMethod]
    public void Calculate_GetSummary_ShouldFormatCorrectly()
    {
        // Arrange
        var character = Character.Create("Test Character");
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(1);
        character.AddComponent(progressionComponent);
        character.AddComponent(AbilitiesComponent.Create(character.Id));

        var registration1 = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Feature A", "ClassFeature");
        registration1.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "test-stat", "+3");

        var registration2 = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Feature B", "ClassFeature");
        registration2.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), "test-stat", "-1");
        var registrations = new List<Registration> { registration1, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        var group = result.GetGroup("test-stat");
        var summary = group.GetSummary();
        summary.Should().Contain("Feature A (+3)");
        summary.Should().Contain("Feature B (-1)");
    }
}
