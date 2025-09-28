using FluentAssertions;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.Skills;

namespace Starlights.Modules.Characters.Tests.Characters;

[TestClass]
public class SkillTests
{
    [TestMethod]
    public void Create_Defaults_AreCorrect()
    {
        // Arrange
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();

        // Act
        var skill = Skill.Create(regId, "Athletics", abilityScoreId, "STR");

        // Assert
        skill.AbilityScoreModifier.Should().Be(0);
        skill.AdditionalBonus.Should().Be(0);
        skill.CalculatedBonus.Should().Be(0);
        skill.AbilityScoreId.Should().Be(abilityScoreId);
        skill.AssociatedRegistrationId.Should().Be(regId);
        skill.Name.Should().Be("Athletics");
        skill.AbilityScoreAbbreviation.Should().Be("STR");
    }

    [TestMethod]
    public void UpdateAbilityScoreModifier_Recalculates_ReturnsTrue()
    {
        // Arrange
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var skill = Skill.Create(regId, "Stealth", abilityScoreId, "DEX");

        // Act
        var updated = skill.UpdateAbilityScoreModifier(3);

        // Assert
        updated.Should().BeTrue();
        skill.AbilityScoreModifier.Should().Be(3);
        skill.CalculatedBonus.Should().Be(3);
    }

    [TestMethod]
    public void UpdateAbilityScoreModifier_NoChange_ReturnsFalse()
    {
        // Arrange
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var skill = Skill.Create(regId, "Stealth", abilityScoreId, "DEX");
        skill.UpdateAbilityScoreModifier(5).Should().BeTrue();
        var currentCalculated = skill.CalculatedBonus;

        // Act
        var updated = skill.UpdateAbilityScoreModifier(5);

        // Assert
        updated.Should().BeFalse();
        skill.CalculatedBonus.Should().Be(currentCalculated);
    }

    [TestMethod]
    public void UpdateAdditionalBonus_Recalculates_ReturnsTrue()
    {
        // Arrange
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var skill = Skill.Create(regId, "Arcana", abilityScoreId, "INT");

        // Act
        var updated = skill.UpdateAdditionalBonus(2);

        // Assert
        updated.Should().BeTrue();
        skill.AdditionalBonus.Should().Be(2);
        skill.CalculatedBonus.Should().Be(2);
    }

    [TestMethod]
    public void UpdateAdditionalBonus_NoChange_ReturnsFalse()
    {
        // Arrange
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var skill = Skill.Create(regId, "Arcana", abilityScoreId, "INT");
        skill.UpdateAdditionalBonus(4).Should().BeTrue();
        var currentCalculated = skill.CalculatedBonus;

        // Act
        var updated = skill.UpdateAdditionalBonus(4);

        // Assert
        updated.Should().BeFalse();
        skill.CalculatedBonus.Should().Be(currentCalculated);
    }

    [TestMethod]
    public void UpdateBoth_Recalculates_Sum()
    {
        // Arrange
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var skill = Skill.Create(regId, "Persuasion", abilityScoreId, "CHA");

        // Act
        skill.UpdateAbilityScoreModifier(4);
        skill.UpdateAdditionalBonus(-1);

        // Assert
        skill.AbilityScoreModifier.Should().Be(4);
        skill.AdditionalBonus.Should().Be(-1);
        skill.CalculatedBonus.Should().Be(3);
    }
}
