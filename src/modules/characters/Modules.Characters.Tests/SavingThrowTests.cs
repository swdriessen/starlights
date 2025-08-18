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
    public void UpdateAbilityScoreModifier_Recalculates()
    {
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var save = SavingThrow.Create(regId, "Dexterity Saving Throw", abilityScoreId, "DEX");

        save.UpdateAbilityScoreModifier(2);

        save.AbilityScoreModifier.Should().Be(2);
        save.CalculatedBonus.Should().Be(2);
    }

    [TestMethod]
    public void UpdateAdditionalBonus_Recalculates()
    {
        var regId = RegistrationId.New();
        var abilityScoreId = AbilityScoreId.New();
        var save = SavingThrow.Create(regId, "Constitution Saving Throw", abilityScoreId, "CON");

        save.UpdateAdditionalBonus(1);

        save.AdditionalBonus.Should().Be(1);
        save.CalculatedBonus.Should().Be(1);
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
