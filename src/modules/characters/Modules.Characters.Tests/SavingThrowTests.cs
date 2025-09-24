using FluentAssertions;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.SavingThrows;

namespace Starlights.Modules.Characters.Tests;

[TestClass]
public class SavingThrowTests
{
    [TestMethod]
    public void Create_Defaults_AreCorrect()
    {
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();

        var save = SavingThrow.Create(regId, "Strength Saving Throw", abilityScoreId, "STR");

        save.AbilityScoreModifier.Should().Be(0);
        save.AdditionalBonus.Should().Be(0);
        save.CalculatedBonus.Should().Be(0);
        save.AbilityScoreId.Should().Be(abilityScoreId);
        save.AssociatedRegistrationId.Should().Be(regId);
        save.Name.Should().Be("Strength Saving Throw");
        save.AbilityScoreAbbreviation.Should().Be("STR");
    }

    [TestMethod]
    public void UpdateAbilityScoreModifier_Recalculates_ReturnsTrue()
    {
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var save = SavingThrow.Create(regId, "Dexterity Saving Throw", abilityScoreId, "DEX");

        var updated = save.UpdateAbilityScoreModifier(2);

        updated.Should().BeTrue();
        save.AbilityScoreModifier.Should().Be(2);
        save.CalculatedBonus.Should().Be(2);
    }

    [TestMethod]
    public void UpdateAbilityScoreModifier_NoChange_ReturnsFalse()
    {
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var save = SavingThrow.Create(regId, "Dexterity Saving Throw", abilityScoreId, "DEX");
        save.UpdateAbilityScoreModifier(5).Should().BeTrue();
        var currentCalculated = save.CalculatedBonus;

        var updated = save.UpdateAbilityScoreModifier(5);

        updated.Should().BeFalse();
        save.CalculatedBonus.Should().Be(currentCalculated);
    }

    [TestMethod]
    public void UpdateAdditionalBonus_Recalculates_ReturnsTrue()
    {
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var save = SavingThrow.Create(regId, "Constitution Saving Throw", abilityScoreId, "CON");

        var updated = save.UpdateAdditionalBonus(1);

        updated.Should().BeTrue();
        save.AdditionalBonus.Should().Be(1);
        save.CalculatedBonus.Should().Be(1);
    }

    [TestMethod]
    public void UpdateAdditionalBonus_NoChange_ReturnsFalse()
    {
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var save = SavingThrow.Create(regId, "Constitution Saving Throw", abilityScoreId, "CON");
        save.UpdateAdditionalBonus(3).Should().BeTrue();
        var currentCalculated = save.CalculatedBonus;

        var updated = save.UpdateAdditionalBonus(3);

        updated.Should().BeFalse();
        save.CalculatedBonus.Should().Be(currentCalculated);
    }

    [TestMethod]
    public void UpdateBoth_Recalculates_Sum()
    {
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var save = SavingThrow.Create(regId, "Wisdom Saving Throw", abilityScoreId, "WIS");

        save.UpdateAbilityScoreModifier(3);
        save.UpdateAdditionalBonus(-1);

        save.AbilityScoreModifier.Should().Be(3);
        save.AdditionalBonus.Should().Be(-1);
        save.CalculatedBonus.Should().Be(2);
    }
}
