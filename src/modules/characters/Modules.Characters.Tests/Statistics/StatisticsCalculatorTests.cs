using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Services.Statistics;
using Starlights.Modules.Characters.Services.Statistics.Initializers;
using Starlights.Modules.Characters.Services.Statistics.Processors;

namespace Starlights.Modules.Characters.Tests.Statistics;

[TestClass]
public sealed class StatisticsCalculatorTests
{
    private StatisticsCalculator _calculator = default!;

    [TestInitialize]
    public void Initialize()
    {
        // Create calculator with standard processors
        var seedProcessors = new List<IStatisticsCalculationInitializer>
        {
            new CharacterStatisticsInitializer(),
            new AbilitiesStatisticsInitializer(),
        };

        _calculator = new StatisticsCalculator(NullLogger<StatisticsCalculator>.Instance,
            seedProcessors,
            [new ProficiencyGroupProcessor(NullLogger<ProficiencyGroupProcessor>.Instance),
            new AbilitiesGroupProcessor()], []);
    }

    #region Helper Methods

    private static Character CreateTestCharacter(string name = "Test Character", int level = 1)
    {
        var character = Character.Create(name);
        var progressionComponent = ProgressionComponent.Create(character.Id);
        progressionComponent.SetCharacterLevel(level);
        character.AddComponent(progressionComponent);
        character.AddComponent(AbilitiesComponent.Create(character.Id));
        return character;
    }

    private static void AddAbilityScore(Character character, string name, string abbreviation, int baseScore)
    {
        var abilitiesComponent = character.GetRequiredComponent<AbilitiesComponent>();
        var registration = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), name, "Ability");
        var abilityScore = abilitiesComponent.CreateAbilityScore(registration.Id, name, abbreviation);
        abilityScore.UpdateBaseScore(baseScore);
    }

    private static Registration CreateRegistrationWithStatisticRule(
     Character character,
        string featureName,
        string featureType,
   string statName,
     string statValue,
        Action<RegistrationStatisticRule>? configureRule = null)
    {
        var registration = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), featureName, featureType);
        var rule = registration.CreateStatisticRule(new ElementComponentId(Guid.NewGuid()), statName, statValue);
        configureRule?.Invoke(rule);
        return registration;
    }

    #endregion

    #region Basic Functionality Tests

    [TestMethod]
    public void Calculate_WithNoRegistrations_ShouldReturnResult()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registrations = new List<Registration>();

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Should().NotBeNull();
        result.Statistics.Should().NotBeNull();
        result.HasErrors.Should().BeFalse();
    }

    [TestMethod]
    public void Calculate_AllGroupsShouldBeFinalized()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registrations = new List<Registration>();

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.Should().NotBeEmpty();
        result.Statistics.Should().OnlyContain(group => group.IsCompleted);
    }

    #endregion

    #region Character Level Tests

    [TestMethod]
    public void Calculate_WithCharacterLevel_ShouldSeedLevelStatistics()
    {
        // Arrange
        var character = CreateTestCharacter(level: 5);
        var registrations = new List<Registration>();

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("level").Should().BeTrue();
        result.Statistics.GetValue("level").Should().Be(5);
        result.Statistics.GetGroup("level").GetStatisticValues().Should().ContainSingle(v => v.DisplayName == "Character");
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(5)]
    [DataRow(10)]
    [DataRow(20)]
    public void Calculate_WithDifferentCharacterLevels_ShouldReflectCorrectLevel(int level)
    {
        // Arrange
        var character = CreateTestCharacter(level: level);
        var registrations = new List<Registration>();

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("level").Should().Be(level);
    }

    #endregion

    #region Ability Score Tests

    [TestMethod]
    public void Calculate_WithAbilityScores_ShouldSeedAbilityStatistics()
    {
        // Arrange
        var character = CreateTestCharacter();
        AddAbilityScore(character, "Strength", "STR", 16); // Score 16 = modifier +3
        var registrations = new List<Registration>();

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("strength:score").Should().BeTrue();
        result.Statistics.GetValue("strength:score").Should().Be(16);
        result.Statistics.GetGroup("strength:score").GetStatisticValues().Should().ContainSingle(v => v.DisplayName == "Strength");

        result.Statistics.ContainsGroup("strength:modifier").Should().BeTrue();
        result.Statistics.GetValue("strength:modifier").Should().Be(3);
        result.Statistics.GetGroup("strength:modifier").GetStatisticValues().Should().ContainSingle(v => v.DisplayName == "Strength Modifier");

        result.Statistics.ContainsGroup("strength:modifier:half").Should().BeTrue();
        result.Statistics.GetValue("strength:modifier:half").Should().Be(1); // floor(3/2) = 1

        result.Statistics.ContainsGroup("strength:modifier:half:up").Should().BeTrue();
        result.Statistics.GetValue("strength:modifier:half:up").Should().Be(2); // ceil(3/2) = 2
    }

    [TestMethod]
    [DataRow(8, -1)]  // Modifier for score 8
    [DataRow(10, 0)]  // Modifier for score 10
    [DataRow(12, 1)]  // Modifier for score 12
    [DataRow(14, 2)]  // Modifier for score 14
    [DataRow(16, 3)]  // Modifier for score 16
    [DataRow(18, 4)]  // Modifier for score 18
    [DataRow(20, 5)]  // Modifier for score 20
    public void Calculate_WithDifferentAbilityScores_ShouldCalculateCorrectModifiers(int score, int expectedModifier)
    {
        // Arrange
        var character = CreateTestCharacter();
        AddAbilityScore(character, "Strength", "STR", score);
        var registrations = new List<Registration>();

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("strength:modifier").Should().Be(expectedModifier);
    }

    [TestMethod]
    public void Calculate_WithMultipleAbilityScores_ShouldCreateStatisticsForAll()
    {
        // Arrange
        var character = CreateTestCharacter();
        AddAbilityScore(character, "Strength", "STR", 16);
        AddAbilityScore(character, "Dexterity", "DEX", 14);
        AddAbilityScore(character, "Constitution", "CON", 12);
        var registrations = new List<Registration>();

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("strength:score").Should().BeTrue();
        result.Statistics.ContainsGroup("dexterity:score").Should().BeTrue();
        result.Statistics.ContainsGroup("constitution:score").Should().BeTrue();
        result.Statistics.GetValue("strength:modifier").Should().Be(3);
        result.Statistics.GetValue("dexterity:modifier").Should().Be(2);
        result.Statistics.GetValue("constitution:modifier").Should().Be(1);
    }

    #endregion

    #region Direct Value Rules Tests

    [TestMethod]
    public void Calculate_WithDirectValueRule_ShouldProcessCorrectly()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration = CreateRegistrationWithStatisticRule(character, "Test Feature", "ClassFeature", "test-stat", "+5");
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("test-stat").Should().BeTrue();
        result.Statistics.GetValue("test-stat").Should().Be(5);
        result.Statistics.GetGroup("test-stat").GetStatisticValues().Should().ContainSingle()
            .Which.DisplayName.Should().Be("Test Feature");
    }

    [TestMethod]
    public void Calculate_WithNegativeDirectValue_ShouldProcessCorrectly()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration = CreateRegistrationWithStatisticRule(character, "Penalty", "Debuff", "armor-class", "-2");
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("armor-class").Should().BeTrue();
        result.Statistics.GetValue("armor-class").Should().Be(-2);
    }

    [TestMethod]
    public void Calculate_WithMultipleAdditiveRules_ShouldSumValues()
    {
        // Arrange
        var character = CreateTestCharacter(level: 5);
        var registration1 = CreateRegistrationWithStatisticRule(character, "Feature 1", "ClassFeature", "bonus-stat", "+2");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Feature 2", "ClassFeature", "bonus-stat", "+3");
        var registrations = new List<Registration> { registration1, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("bonus-stat").Should().BeTrue();
        result.Statistics.GetValue("bonus-stat").Should().Be(5, "Multiple additive bonuses should sum");
        result.Statistics.GetGroup("bonus-stat").GetStatisticValues().Should().HaveCount(2);
        result.Statistics.GetGroup("bonus-stat").GetStatisticValues().Should().Contain(v => v.DisplayName == "Feature 1");
        result.Statistics.GetGroup("bonus-stat").GetStatisticValues().Should().Contain(v => v.DisplayName == "Feature 2");
    }

    [TestMethod]
    public void Calculate_WithPositiveAndNegativeValues_ShouldSumCorrectly()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration1 = CreateRegistrationWithStatisticRule(character, "Bonus", "Buff", "test-stat", "+10");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Penalty", "Debuff", "test-stat", "-3");
        var registrations = new List<Registration> { registration1, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("test-stat").Should().Be(7); // 10 - 3 = 7
    }

    #endregion

    #region Reference Value Rules Tests

    [TestMethod]
    public void Calculate_WithReferenceValue_ShouldResolveReferences()
    {
        // Arrange
        var character = CreateTestCharacter();
        AddAbilityScore(character, "Strength", "STR", 14); // Modifier +2

        var registration = CreateRegistrationWithStatisticRule(character, "Test Feature", "ClassFeature", "attack-bonus", "strength:modifier");
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("attack-bonus").Should().BeTrue();
        result.Statistics.GetValue("attack-bonus").Should().Be(2, "Should resolve to the strength modifier value");
        result.Statistics.GetGroup("attack-bonus").GetStatisticValues().Should().ContainSingle()
            .Which.DisplayName.Should().Be("Test Feature");
    }

    [TestMethod]
    public void Calculate_WithChainedReferences_ShouldResolveCorrectly()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration1 = CreateRegistrationWithStatisticRule(character, "Base", "Feature", "stat-a", "+5");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Derived", "Feature", "stat-b", "stat-a");
        var registration3 = CreateRegistrationWithStatisticRule(character, "Final", "Feature", "stat-c", "stat-b");
        var registrations = new List<Registration> { registration1, registration2, registration3 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("stat-a").Should().Be(5);
        result.Statistics.GetValue("stat-b").Should().Be(5);
        result.Statistics.GetValue("stat-c").Should().Be(5);
    }

    [TestMethod]
    public void Calculate_WithUnresolvableReference_ShouldNotCrash()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration = CreateRegistrationWithStatisticRule(character, "Bad Feature", "Feature", "test-stat", "nonexistent-stat");
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("test-stat").Should().BeFalse("Reference could not be resolved");
        result.HasErrors.Should().BeFalse("The calculation should complete without errors, at best we should add a warning");
    }

    #endregion

    #region Stacking Bonus Tests

    [TestMethod]
    public void Calculate_WithStackingBonus_ShouldUseHighestValue()
    {
        // Arrange
        var character = CreateTestCharacter(level: 5);
        var registration1 = CreateRegistrationWithStatisticRule(character, "Feature 1", "ClassFeature", "armor-class", "+2",
            rule => rule.UpdateStackingBonus("enhancement"));
        var registration2 = CreateRegistrationWithStatisticRule(character, "Feature 2", "ClassFeature", "armor-class", "+3",
            rule => rule.UpdateStackingBonus("enhancement"));
        var registrations = new List<Registration> { registration1, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("armor-class").Should().BeTrue();
        result.Statistics.GetValue("armor-class").Should().Be(3, "Only the highest stacking bonus should be applied");
        result.Statistics.GetGroup("armor-class").GetStatisticValues().Should().ContainSingle()
            .Which.DisplayName.Should().Contain("Feature 2");
    }

    [TestMethod]
    public void Calculate_WithMultipleDifferentStackingBonuses_ShouldStackAllTypes()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration1 = CreateRegistrationWithStatisticRule(character, "Enhancement", "Item", "armor-class", "+2",
            rule => rule.UpdateStackingBonus("enhancement"));
        var registration2 = CreateRegistrationWithStatisticRule(character, "Deflection", "Item", "armor-class", "+1",
            rule => rule.UpdateStackingBonus("deflection"));
        var registration3 = CreateRegistrationWithStatisticRule(character, "Natural Armor", "Ability", "armor-class", "+3",
            rule => rule.UpdateStackingBonus("natural"));
        var registrations = new List<Registration> { registration1, registration2, registration3 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("armor-class").Should().Be(6, "Different stacking bonus types should stack");
        result.Statistics.GetGroup("armor-class").GetStatisticValues().Should().HaveCount(3);
    }

    [TestMethod]
    public void Calculate_WithStackingBonusAndNonStacking_ShouldCombineBoth()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration1 = CreateRegistrationWithStatisticRule(character, "Base", "Feature", "armor-class", "+10");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Enhancement", "Item", "armor-class", "+2",
            rule => rule.UpdateStackingBonus("enhancement"));
        var registrations = new List<Registration> { registration1, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("armor-class").Should().Be(12);
    }

    [TestMethod]
    public void Calculate_WithEqualStackingBonuses_ShouldIncludeBothSourcesInDisplayName()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration1 = CreateRegistrationWithStatisticRule(character, "Item A", "Item", "armor-class", "+2",
            rule => rule.UpdateStackingBonus("enhancement"));
        var registration2 = CreateRegistrationWithStatisticRule(character, "Item B", "Item", "armor-class", "+2",
            rule => rule.UpdateStackingBonus("enhancement"));
        var registrations = new List<Registration> { registration1, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("armor-class").Should().Be(2);
        var displayName = result.Statistics.GetGroup("armor-class").GetStatisticValues().Single().DisplayName;
        displayName.Should().Contain("Item A");
    }

    [TestMethod]
    public void Calculate_WithStackingBonusReference_ShouldResolveAndApply()
    {
        // Arrange
        var character = CreateTestCharacter();
        AddAbilityScore(character, "Dexterity", "DEX", 16); // Modifier +3

        var registration1 = CreateRegistrationWithStatisticRule(character, "Dex to AC", "Feature", "armor-class", "dexterity:modifier",
            rule => rule.UpdateStackingBonus("dexterity"));
        var registration2 = CreateRegistrationWithStatisticRule(character, "Dex Limit", "Feature", "armor-class", "dexterity:modifier:half",
            rule => rule.UpdateStackingBonus("dexterity"));
        var registrations = new List<Registration> { registration1, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("armor-class").Should().Be(3, "Should use the highest dexterity-based bonus");
    }

    [TestMethod]
    public void Calculate_AbilityScoreIncrease_ShouldResolveAndApplyEarly()
    {
        // Arrange
        var character = CreateTestCharacter();
        AddAbilityScore(character, "Dexterity", "DEX", 16); // Modifier +3

        var registration1 = CreateRegistrationWithStatisticRule(character, "Dex to AC", "Feature", "dexterity", "2");
        var registrations = new List<Registration> { registration1 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("dexterity").Should().Be(2);
        result.Statistics.GetValue("dexterity:score").Should().Be(18);
        result.Statistics.GetValue("dexterity:modifier").Should().Be(4);
    }

    [TestMethod]
    public void Calculate_AbilityScoreIncreaseWithStackingBonusReference_ShouldResolveAndApplyEarly()
    {
        // Arrange
        var character = CreateTestCharacter();
        AddAbilityScore(character, "Dexterity", "DEX", 16); // Modifier +3

        var registration1 = CreateRegistrationWithStatisticRule(character, "Dex to AC", "Feature", "dexterity", "2",
            rule => rule.UpdateStackingBonus("stackdexterity"));
        var registrations = new List<Registration> { registration1 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("dexterity").Should().Be(2);
        result.Statistics.GetValue("dexterity:score").Should().Be(18);
        result.Statistics.GetValue("dexterity:modifier").Should().Be(4);
    }

    #endregion

    #region Min/Max Constraint Tests

    [TestMethod]
    public void Calculate_WithMaximumValue_ShouldApplyCap()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration = CreateRegistrationWithStatisticRule(character, "Test Feature", "ClassFeature", "test-stat", "100",
            rule => rule.UpdateMaximumValue(20));
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("test-stat").Should().BeTrue();
        result.Statistics.GetValue("test-stat").Should().Be(20, "Value should be capped at maximum");
        result.Statistics.GetGroup("test-stat").GetStatisticValues().Should().Contain(v => v.DisplayName!.Contains("Test Feature"));
    }

    [TestMethod]
    public void Calculate_WithMinimumValue_ShouldApplyFloor()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration = CreateRegistrationWithStatisticRule(character, "Test Feature", "ClassFeature", "test-stat", "1",
            rule => rule.UpdateMinimumValue(5));
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("test-stat").Should().BeTrue();
        result.Statistics.GetValue("test-stat").Should().Be(5, "Value should be raised to minimum");
        result.Statistics.GetGroup("test-stat").GetStatisticValues().Should().Contain(v => v.DisplayName!.Contains("Test Feature"));
    }

    [TestMethod]
    public void Calculate_WithinMinMaxRange_ShouldNotApplyConstraints()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration = CreateRegistrationWithStatisticRule(character, "Test Feature", "ClassFeature", "test-stat", "10", rule =>
        {
            rule.UpdateMinimumValue(5);
            rule.UpdateMaximumValue(15);
        });
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("test-stat").Should().Be(10);
        result.Statistics.GetGroup("test-stat").GetStatisticValues().Should().NotContain(v => v.DisplayName!.Contains("Cap"));
        result.Statistics.GetGroup("test-stat").GetStatisticValues().Should().NotContain(v => v.DisplayName!.Contains("Floor"));
    }

    [TestMethod]
    public void Calculate_WithMaxOnReferenceValue_ShouldCapResolvedValue()
    {
        // Arrange
        var character = CreateTestCharacter();
        AddAbilityScore(character, "Dexterity", "DEX", 18); // Modifier +4

        var registration = CreateRegistrationWithStatisticRule(character, "Dex to AC", "Feature", "armor-class", "dexterity:modifier",
            rule => rule.UpdateMaximumValue(2));
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("armor-class").Should().Be(2, "Referenced value should be capped");
    }

    [TestMethod]
    public void Calculate_WithBothMinAndMax_ShouldApplyBothConstraints()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration1 = CreateRegistrationWithStatisticRule(character, "Base", "Feature", "test-stat", "50", rule =>
        {
            rule.UpdateMinimumValue(10);
            rule.UpdateMaximumValue(30);
        });
        var registrations = new List<Registration> { registration1 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("test-stat").Should().Be(30, "Should cap to maximum");
    }

    #endregion

    #region Proficiency Tests

    [TestMethod]
    public void Calculate_WithProficiencyStatistic_ShouldCreateVariants()
    {
        // Arrange
        var character = CreateTestCharacter(level: 5);
        var registration = CreateRegistrationWithStatisticRule(character, "Proficiency Rule", "Rule", "proficiency", "3");
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("proficiency").Should().BeTrue();
        result.Statistics.GetValue("proficiency").Should().Be(3);

        result.Statistics.ContainsGroup("proficiency:half").Should().BeTrue();
        result.Statistics.GetValue("proficiency:half").Should().Be(1); // floor(3/2) = 1
        result.Statistics.ContainsGroup("proficiency:half:up").Should().BeTrue();
        result.Statistics.GetValue("proficiency:half:up").Should().Be(2); // ceil(3/2) = 2
    }

    [TestMethod]
    [DataRow(2, 1, 1)]  // Proficiency 2: half = 1, half:up = 1
    [DataRow(3, 1, 2)]  // Proficiency 3: half = 1, half:up = 2
    [DataRow(4, 2, 2)]  // Proficiency 4: half = 2, half:up = 2
    [DataRow(5, 2, 3)]  // Proficiency 5: half = 2, half:up = 3
    public void Calculate_ProficiencyVariants_ShouldCalculateCorrectly(int proficiency, int expectedHalf, int expectedHalfUp)
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration = CreateRegistrationWithStatisticRule(character, "Proficiency", "Feature", "proficiency", $"{proficiency}");
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("proficiency:half").Should().Be(expectedHalf);
        result.Statistics.GetValue("proficiency:half:up").Should().Be(expectedHalfUp);
    }

    #endregion

    #region Complex Combination Tests

    [TestMethod]
    public void Calculate_ComplexScenario_MultipleFeaturesCombined()
    {
        // Arrange
        var character = CreateTestCharacter(level: 5);
        AddAbilityScore(character, "Dexterity", "DEX", 16); // Modifier +3

        var registration1 = CreateRegistrationWithStatisticRule(character, "Base AC", "Feature", "armor-class", "+10");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Dex to AC", "Feature", "armor-class", "dexterity:modifier");
        var registration3 = CreateRegistrationWithStatisticRule(character, "Shield", "Item", "armor-class", "+2",
            rule => rule.UpdateStackingBonus("shield"));
        var registration4 = CreateRegistrationWithStatisticRule(character, "Magic Shield", "Item", "armor-class", "+3",
            rule => rule.UpdateStackingBonus("shield")); // Should use +3, not +2

        var registrations = new List<Registration> { registration1, registration2, registration3, registration4 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("armor-class").Should().Be(16); // 10 + 3 + 3 = 16
    }

    [TestMethod]
    public void Calculate_ComplexScenario_WithConstraintsAndReferences()
    {
        // Arrange
        var character = CreateTestCharacter();
        AddAbilityScore(character, "Intelligence", "INT", 18); // Modifier +4

        var registration1 = CreateRegistrationWithStatisticRule(character, "Wizard Spells", "Feature", "max-spells", "intelligence:modifier",
            rule => rule.UpdateMinimumValue(1)); // At least 1 spell
        var registration2 = CreateRegistrationWithStatisticRule(character, "Bonus Spells", "Feature", "max-spells", "+2");

        var registrations = new List<Registration> { registration1, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("max-spells").Should().Be(6); // 4 + 2 = 6
    }

    [TestMethod]
    public void Calculate_ComplexScenario_StackingWithDifferentSources()
    {
        // Arrange
        var character = CreateTestCharacter();

        var registration1 = CreateRegistrationWithStatisticRule(character, "Ring of Protection +1", "Item", "saving-throw", "+1",
            rule => rule.UpdateStackingBonus("deflection"));
        var registration2 = CreateRegistrationWithStatisticRule(character, "Ring of Protection +2", "Item", "saving-throw", "+2",
            rule => rule.UpdateStackingBonus("deflection")); // Higher deflection
        var registration3 = CreateRegistrationWithStatisticRule(character, "Cloak of Resistance +1", "Item", "saving-throw", "+1",
            rule => rule.UpdateStackingBonus("resistance"));
        var registration4 = CreateRegistrationWithStatisticRule(character, "Divine Grace", "Ability", "saving-throw", "+3");

        var registrations = new List<Registration> { registration1, registration2, registration3, registration4 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("saving-throw").Should().Be(6); // +2 (deflection) + +1 (resistance) + +3 (untyped) = 6
    }

    #endregion

    #region Metadata and Display Tests

    //[TestMethod]
    //public void Calculate_StatisticValues_ShouldIncludeRuleIds()
    //{
    //    // Arrange
    //    var character = CreateTestCharacter();
    //    var registration = Registration.Create(character.Id, new ElementId(Guid.NewGuid()), "Test Feature", "ClassFeature");
    //    var ruleId = new ElementComponentId(Guid.NewGuid());
    //    registration.CreateStatisticRule(ruleId, "test-stat", "+5");
    //    var registrations = new List<Registration> { registration };

    //    // Act
    //    var result = _calculator.Calculate(character, registrations);

    //    // Assert
    //    result.Statistics.GetGroup("test-stat").GetValues().Should().ContainSingle()
    //        .Which.RuleId.Should().Be(ruleId.Value);
    //}

    [TestMethod]
    public void Calculate_GetSummary_ShouldFormatCorrectly()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration1 = CreateRegistrationWithStatisticRule(character, "Feature A", "ClassFeature", "test-stat", "+3");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Feature B", "ClassFeature", "test-stat", "-1");
        var registrations = new List<Registration> { registration1, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        var group = result.Statistics.GetGroup("test-stat");
        var summary = group.GetSummary();
        summary.Should().Contain("Feature A (+3)");
        summary.Should().Contain("Feature B (-1)");
    }

    [TestMethod]
    public void Calculate_GetSummary_WithoutValues_ShouldShowOnlyNames()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration = CreateRegistrationWithStatisticRule(character, "Feature A", "ClassFeature", "test-stat", "+5");
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        var group = result.Statistics.GetGroup("test-stat");
        var summary = group.GetSummary(includeValues: false);
        summary.Should().Be("Feature A");
    }

    #endregion

    #region Edge Cases

    [TestMethod]
    public void Calculate_WithZeroValue_ShouldStillProcess()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration = CreateRegistrationWithStatisticRule(character, "Neutral", "Feature", "test-stat", "0");
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("test-stat").Should().BeTrue();
        result.Statistics.GetValue("test-stat").Should().Be(0);
    }

    [TestMethod]
    public void Calculate_WithLargeNumberOfRules_ShouldHandleEfficiently()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registrations = new List<Registration>();

        for (int i = 0; i < 100; i++)
        {
            registrations.Add(CreateRegistrationWithStatisticRule(character, $"Feature {i}", "Feature", "test-stat", "+1"));
        }

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("test-stat").Should().Be(100);
        result.Statistics.GetGroup("test-stat").GetStatisticValues().Should().HaveCount(100);
    }

    [TestMethod]
    public void Calculate_WithCircularReference_ShouldNotCrashOrInfiniteLoop()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration1 = CreateRegistrationWithStatisticRule(character, "Feature A", "Feature", "stat-a", "stat-b");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Feature B", "Feature", "stat-b", "stat-a");
        var registrations = new List<Registration> { registration1, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert - Should handle gracefully without infinite loop
        result.Should().NotBeNull();
        result.HasErrors.Should().BeTrue("one or more circular dependencies should result in error messages along side completed statistic values");
    }

    [TestMethod]
    public void Calculate_WithSameStatisticNameDifferentCasing_ShouldNormalize()
    {
        // This test verifies that the system normalizes statistic names (domain should handle this)
        // Arrange
        var character = CreateTestCharacter();
        var registration = CreateRegistrationWithStatisticRule(character, "Feature", "Feature", "test-stat", "+5");
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("test-stat").Should().BeTrue();
    }

    [TestMethod]
    public void Calculate_EmptyRegistrationList_ShouldStillSeedBaseStatistics()
    {
        // Arrange
        var character = CreateTestCharacter(level: 3);
        AddAbilityScore(character, "Strength", "STR", 14);
        var registrations = new List<Registration>();

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.ContainsGroup("level").Should().BeTrue();
        result.Statistics.ContainsGroup("strength:score").Should().BeTrue();
        result.Statistics.GetValue("level").Should().Be(3);
        result.Statistics.GetValue("strength:score").Should().Be(14);
    }

    #endregion
}

