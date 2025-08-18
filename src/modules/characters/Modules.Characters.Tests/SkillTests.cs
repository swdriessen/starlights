using FluentAssertions;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.Skills;

namespace Starlights.Modules.Characters.Tests;

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
    public void UpdateAbilityScoreModifier_Recalculates()
    {
        // Arrange
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var skill = Skill.Create(regId, "Stealth", abilityScoreId, "DEX");

        // Act
        skill.UpdateAbilityScoreModifier(3);

        // Assert
        skill.AbilityScoreModifier.Should().Be(3);
        skill.CalculatedBonus.Should().Be(3);
    }

    [TestMethod]
    public void UpdateAdditionalBonus_Recalculates()
    {
        // Arrange
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var skill = Skill.Create(regId, "Arcana", abilityScoreId, "INT");

        // Act
        skill.UpdateAdditionalBonus(2);

        // Assert
        skill.AdditionalBonus.Should().Be(2);
        skill.CalculatedBonus.Should().Be(2);
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
