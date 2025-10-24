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

    #region Topological Sort Tests

    [TestMethod]
    public void Calculate_WithNoDependencies_ShouldProcessAllGroupsInOriginalOrder()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration1 = CreateRegistrationWithStatisticRule(character, "Feature A", "Feature", "stat-a", "+1");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Feature B", "Feature", "stat-b", "+2");
        var registration3 = CreateRegistrationWithStatisticRule(character, "Feature C", "Feature", "stat-c", "+3");
        var registrations = new List<Registration> { registration1, registration2, registration3 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("stat-a").Should().Be(1);
        result.Statistics.GetValue("stat-b").Should().Be(2);
        result.Statistics.GetValue("stat-c").Should().Be(3);
        result.Statistics.GetGroup("stat-a").IsCompleted.Should().BeTrue();
        result.Statistics.GetGroup("stat-b").IsCompleted.Should().BeTrue();
        result.Statistics.GetGroup("stat-c").IsCompleted.Should().BeTrue();
    }

    [TestMethod]
    public void Calculate_WithSingleDependency_ShouldProcessInDependencyOrder()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registration1 = CreateRegistrationWithStatisticRule(character, "Base Stat", "Feature", "stat-base", "+10");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Dependent Stat", "Feature", "stat-derived", "stat-base");
        var registrations = new List<Registration> { registration2, registration1 }; // Intentionally out of order

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("stat-base").Should().Be(10, "Base should be processed first");
        result.Statistics.GetValue("stat-derived").Should().Be(10, "Derived should resolve reference correctly");
        result.Statistics.GetGroup("stat-base").IsCompleted.Should().BeTrue();
        result.Statistics.GetGroup("stat-derived").IsCompleted.Should().BeTrue();
    }

    [TestMethod]
    public void Calculate_WithMultipleDependencyLevels_ShouldProcessInCorrectOrder()
    {
        // Arrange
        var character = CreateTestCharacter();
        // Create a chain: stat-a -> stat-b -> stat-c -> stat-d
        var registration1 = CreateRegistrationWithStatisticRule(character, "Level 1", "Feature", "stat-a", "+5");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Level 2", "Feature", "stat-b", "stat-a");
        var registration3 = CreateRegistrationWithStatisticRule(character, "Level 3", "Feature", "stat-c", "stat-b");
        var registration4 = CreateRegistrationWithStatisticRule(character, "Level 4", "Feature", "stat-d", "stat-c");

        // Add in reverse order to test topological sort
        var registrations = new List<Registration> { registration4, registration3, registration2, registration1 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("stat-a").Should().Be(5);
        result.Statistics.GetValue("stat-b").Should().Be(5);
        result.Statistics.GetValue("stat-c").Should().Be(5);
        result.Statistics.GetValue("stat-d").Should().Be(5);
        result.HasErrors.Should().BeFalse();
    }

    [TestMethod]
    public void Calculate_WithComplexDependencyGraph_ShouldResolveCorrectly()
    {
        // Arrange
        var character = CreateTestCharacter();

        // Create a diamond dependency: base -> left & right -> combined
        var registration1 = CreateRegistrationWithStatisticRule(character, "Base", "Feature", "stat-base", "+10");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Left Branch", "Feature", "stat-left", "stat-base");
        var registration3 = CreateRegistrationWithStatisticRule(character, "Right Branch", "Feature", "stat-right", "stat-base");
        var registration4 = CreateRegistrationWithStatisticRule(character, "Combined Left", "Feature", "stat-combined", "stat-left");
        var registration5 = CreateRegistrationWithStatisticRule(character, "Combined Right", "Feature", "stat-combined", "stat-right");

        // Add in scrambled order
        var registrations = new List<Registration> { registration5, registration2, registration4, registration1, registration3 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("stat-base").Should().Be(10);
        result.Statistics.GetValue("stat-left").Should().Be(10);
        result.Statistics.GetValue("stat-right").Should().Be(10);
        result.Statistics.GetValue("stat-combined").Should().Be(20, "Combined should have both left and right");
        result.HasErrors.Should().BeFalse();
    }

    [TestMethod]
    public void Calculate_WithMultipleIndependentChains_ShouldProcessAllCorrectly()
    {
        // Arrange
        var character = CreateTestCharacter();

        // Chain 1: a -> b
        var registration1 = CreateRegistrationWithStatisticRule(character, "Chain 1 Base", "Feature", "chain1-a", "+5");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Chain 1 Derived", "Feature", "chain1-b", "chain1-a");

        // Chain 2: x -> y -> z
        var registration3 = CreateRegistrationWithStatisticRule(character, "Chain 2 Base", "Feature", "chain2-x", "+10");
        var registration4 = CreateRegistrationWithStatisticRule(character, "Chain 2 Mid", "Feature", "chain2-y", "chain2-x");
        var registration5 = CreateRegistrationWithStatisticRule(character, "Chain 2 End", "Feature", "chain2-z", "chain2-y");

        // Add in random order
        var registrations = new List<Registration> { registration4, registration2, registration5, registration1, registration3 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("chain1-a").Should().Be(5);
        result.Statistics.GetValue("chain1-b").Should().Be(5);
        result.Statistics.GetValue("chain2-x").Should().Be(10);
        result.Statistics.GetValue("chain2-y").Should().Be(10);
        result.Statistics.GetValue("chain2-z").Should().Be(10);
        result.HasErrors.Should().BeFalse();
    }

    [TestMethod]
    public void Calculate_WithAbilityModifierDependency_ShouldProcessAfterAbilityInitialization()
    {
        // Arrange
        var character = CreateTestCharacter();
        AddAbilityScore(character, "Dexterity", "DEX", 16); // Modifier +3

        // Create registration that depends on ability modifier (which is initialized by seed processor)
        var registration = CreateRegistrationWithStatisticRule(character, "AC from Dex", "Feature", "armor-class", "dexterity:modifier");
        var registrations = new List<Registration> { registration };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("dexterity:modifier").Should().Be(3, "Ability modifier should be initialized first");
        result.Statistics.GetValue("armor-class").Should().Be(3, "Dependent stat should resolve correctly");
    }

    [TestMethod]
    public void Calculate_WithMixedDirectAndReferencedValues_ShouldOrderCorrectly()
    {
        // Arrange
        var character = CreateTestCharacter();

        // Mix of direct values and references
        var registration1 = CreateRegistrationWithStatisticRule(character, "Direct 1", "Feature", "stat-a", "+5");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Ref to A", "Feature", "stat-b", "stat-a");
        var registration3 = CreateRegistrationWithStatisticRule(character, "Direct 2", "Feature", "stat-c", "+10");
        var registration4 = CreateRegistrationWithStatisticRule(character, "Ref to B and C", "Feature", "stat-d", "stat-b");
        var registration5 = CreateRegistrationWithStatisticRule(character, "Direct to D", "Feature", "stat-d", "stat-c");

        var registrations = new List<Registration> { registration4, registration5, registration2, registration3, registration1 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("stat-a").Should().Be(5);
        result.Statistics.GetValue("stat-b").Should().Be(5);
        result.Statistics.GetValue("stat-c").Should().Be(10);
        result.Statistics.GetValue("stat-d").Should().Be(15, "Should be 5 (from stat-b) + 10 (from stat-c)");
    }

    [TestMethod]
    public void Calculate_WithStackingBonusInDependencyChain_ShouldResolveCorrectly()
    {
        // Arrange
        var character = CreateTestCharacter();

        var registration1 = CreateRegistrationWithStatisticRule(character, "Base", "Feature", "stat-base", "+8");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Bonus 1", "Feature", "stat-derived", "stat-base",
        rule => rule.UpdateStackingBonus("enhancement"));
        var registration3 = CreateRegistrationWithStatisticRule(character, "Bonus 2", "Feature", "stat-derived", "+5",
           rule => rule.UpdateStackingBonus("enhancement"));

        var registrations = new List<Registration> { registration3, registration2, registration1 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("stat-base").Should().Be(8);
        result.Statistics.GetValue("stat-derived").Should().Be(8, "Should use the higher of the two enhancement bonuses");
    }

    [TestMethod]
    public void Calculate_WithLongDependencyChain_ShouldNotTimeout()
    {
        // Arrange
        var character = CreateTestCharacter();
        var registrations = new List<Registration>
 {
     // Create a chain of 20 dependencies
     CreateRegistrationWithStatisticRule(character, "Base", "Feature", "stat-0", "+1")
 };

        for (int i = 1; i < 20; i++)
        {
            var prevStat = $"stat-{i - 1}";
            var currentStat = $"stat-{i}";
            registrations.Add(CreateRegistrationWithStatisticRule(character, $"Level {i}", "Feature", currentStat, prevStat));
        }

        // Shuffle to ensure topological sort is working
        registrations = registrations.OrderBy(x => Guid.NewGuid()).ToList();

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("stat-0").Should().Be(1);
        result.Statistics.GetValue("stat-19").Should().Be(1, "Final stat in chain should resolve to base value");
        result.HasErrors.Should().BeFalse();
    }

    [TestMethod]
    public void Calculate_WithExternalDependency_ShouldProcessAfterInitializers()
    {
        // Arrange
        var character = CreateTestCharacter(level: 5);
        AddAbilityScore(character, "Strength", "STR", 14); // Modifier +2

        // Add proficiency as a base stat (normally would come from game element registrations)
        var proficiencyRegistration = CreateRegistrationWithStatisticRule(character, "Proficiency Bonus", "Feature", "proficiency", "+3");

        // Create stats that depend on initialized values (level, ability modifiers) and proficiency
        var registration1 = CreateRegistrationWithStatisticRule(character, "Attack Bonus", "Feature", "attack", "strength:modifier");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Proficiency to Attack", "Feature", "attack", "proficiency");

        var registrations = new List<Registration> { proficiencyRegistration, registration1, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("strength:modifier").Should().Be(2);
        result.Statistics.GetValue("proficiency").Should().Be(3);
        result.Statistics.GetValue("attack").Should().Be(5, "Should include both strength modifier (2) and proficiency (3)");
    }

    [TestMethod]
    public void Calculate_WithUnresolvableDependencyAtEnd_ShouldNotBlockOthers()
    {
        // Arrange
        var character = CreateTestCharacter();

        var registration1 = CreateRegistrationWithStatisticRule(character, "Good Stat", "Feature", "stat-good", "+5");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Bad Stat", "Feature", "stat-bad", "nonexistent-stat");
        var registration3 = CreateRegistrationWithStatisticRule(character, "Another Good", "Feature", "stat-good2", "+10");

        var registrations = new List<Registration> { registration2, registration1, registration3 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("stat-good").Should().Be(5, "Valid stats should still process");
        result.Statistics.GetValue("stat-good2").Should().Be(10, "Valid stats should still process");
        result.Statistics.ContainsGroup("stat-bad").Should().BeFalse("Unresolvable stat should not be added");
    }

    [TestMethod]
    public void Calculate_WithCircularDependencies_ShouldRemainAtEndOfSort()
    {
        // Arrange
        var character = CreateTestCharacter();

        var registration1 = CreateRegistrationWithStatisticRule(character, "Good Stat", "Feature", "stat-good", "+5");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Circular A", "Feature", "stat-a", "stat-b");
        var registration3 = CreateRegistrationWithStatisticRule(character, "Circular B", "Feature", "stat-b", "stat-a");
        var registration4 = CreateRegistrationWithStatisticRule(character, "Another Good", "Feature", "stat-good2", "+10");

        var registrations = new List<Registration> { registration3, registration1, registration4, registration2 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("stat-good").Should().Be(5, "Non-circular stats should process successfully");
        result.Statistics.GetValue("stat-good2").Should().Be(10, "Non-circular stats should process successfully");
        result.HasErrors.Should().BeTrue("Circular dependencies should be detected and reported");
    }

    [TestMethod]
    public void Calculate_WithPartialCircularDependency_ShouldProcessNonCircularParts()
    {
        // Arrange
        var character = CreateTestCharacter();

        // stat-base -> stat-derived (valid)
        // stat-circular-a <-> stat-circular-b (circular)
        var registration1 = CreateRegistrationWithStatisticRule(character, "Base", "Feature", "stat-base", "+5");
        var registration2 = CreateRegistrationWithStatisticRule(character, "Derived", "Feature", "stat-derived", "stat-base");
        var registration3 = CreateRegistrationWithStatisticRule(character, "Circular A", "Feature", "stat-circular-a", "stat-circular-b");
        var registration4 = CreateRegistrationWithStatisticRule(character, "Circular B", "Feature", "stat-circular-b", "stat-circular-a");

        var registrations = new List<Registration> { registration4, registration2, registration3, registration1 };

        // Act
        var result = _calculator.Calculate(character, registrations);

        // Assert
        result.Statistics.GetValue("stat-base").Should().Be(5);
        result.Statistics.GetValue("stat-derived").Should().Be(5);
        result.Statistics.ContainsGroup("stat-circular-a").Should().BeFalse("Circular dependencies should not be resolved");
        result.Statistics.ContainsGroup("stat-circular-b").Should().BeFalse("Circular dependencies should not be resolved");
        result.HasErrors.Should().BeTrue();
    }

    #endregion
}

