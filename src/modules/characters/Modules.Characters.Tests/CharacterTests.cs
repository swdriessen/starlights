using FluentAssertions;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.SavingThrows.Eventing;
using Starlights.Modules.Characters.Domain.Skills.Eventing;

namespace Starlights.Modules.Characters.Tests;

[TestClass]
public sealed class CharacterTests
{
    [TestMethod]
    public void CreateCharacter()
    {
        // Arrange
        const string name = "Test Character";

        // Act
        var character = Character.Create(name);

        // Assert
        character.Name.Should().Be(name);
        character.Id.Value.Should().NotBeEmpty();
    }

    [TestMethod]
    public void UpdateAbilityBaseScore_ShouldUpdateAbilityAndCascadeToSkillAndSavingThrow()
    {
        // Arrange
        var character = Character.Create("Test");
        var registrationId = RegistrationId.New();
        var str = character.CreateAbilityScore(registrationId, "Strength", "STR");
        var skill = character.CreateSkill(registrationId, "Athletics", str.Id, str.Abbreviation);
        var save = character.CreateSavingThrow(registrationId, "Strength Save", str.Id, str.Abbreviation);
        character.ClearDomainEvents(); // clear creation events for a clean slate

        // Act
        character.UpdateAbilityBaseScore(str.Id, 18); // modifier should become +4

        // Assert ability
        str.BaseScore.Should().Be(18);
        str.CalculatedScore.Should().Be(18); // additional remains 0
        str.CalculatedModifier.Should().Be(4);

        // Assert cascading updates
        skill.AbilityScoreModifier.Should().Be(4);
        save.AbilityScoreModifier.Should().Be(4);

        // Domain events: 1 AbilityScoreUpdated + 1 SkillUpdated + 1 SavingThrowUpdated
        character.DomainEvents.Should().HaveCount(3);
        character.DomainEvents.OfType<AbilityScoreUpdatedEvent>().Should().ContainSingle(e => e.AbilityScoreId == str.Id);
        character.DomainEvents.OfType<SkillUpdatedEvent>().Should().ContainSingle(e => e.SkillId == skill.Id);
        character.DomainEvents.OfType<SavingThrowUpdatedEvent>().Should().ContainSingle(e => e.SavingThrowId == save.Id);
    }

    [TestMethod]
    public void UpdateAbilityBaseScore_ShouldThrow_WhenOutOfRange()
    {
        // Arrange
        var character = Character.Create("Test");
        var registrationId = RegistrationId.New();
        var str = character.CreateAbilityScore(registrationId, "Strength", "STR");

        // Act
        var actLow = () => character.UpdateAbilityBaseScore(str.Id, 0);
        var actHigh = () => character.UpdateAbilityBaseScore(str.Id, 21);

        // Assert
        actLow.Should().Throw<ArgumentOutOfRangeException>();
        actHigh.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void UpdateAbilityAdditionalScore_ShouldAdjustCalculatedValuesAndCascade()
    {
        // Arrange
        var character = Character.Create("Test");
        var registrationId = RegistrationId.New();
        var dex = character.CreateAbilityScore(registrationId, "Dexterity", "DEX"); // base 10 mod 0
        var skill = character.CreateSkill(registrationId, "Stealth", dex.Id, dex.Abbreviation);
        var save = character.CreateSavingThrow(registrationId, "Dex Save", dex.Id, dex.Abbreviation);
        character.ClearDomainEvents();

        // Act - add +2 additional (score 12, mod +1)
        character.UpdateAbilityAdditionalScore(dex.Id, 2);

        // Assert
        dex.AdditionalScore.Should().Be(2);
        dex.CalculatedScore.Should().Be(12);
        dex.CalculatedModifier.Should().Be(1);
        skill.AbilityScoreModifier.Should().Be(1);
        save.AbilityScoreModifier.Should().Be(1);
        character.DomainEvents.Should().HaveCount(3);
        character.DomainEvents.OfType<AbilityScoreUpdatedEvent>().Should().ContainSingle(e => e.AbilityScoreId == dex.Id);
        character.DomainEvents.OfType<SkillUpdatedEvent>().Should().ContainSingle(e => e.SkillId == skill.Id);
        character.DomainEvents.OfType<SavingThrowUpdatedEvent>().Should().ContainSingle(e => e.SavingThrowId == save.Id);
    }

    [TestMethod]
    public void UpdateAbilityAdditionalScore_CanApplyNegativePenalty()
    {
        // Arrange
        var character = Character.Create("Test");
        var registrationId = RegistrationId.New();
        var intScore = character.CreateAbilityScore(registrationId, "Intelligence", "INT"); // base 10 mod 0
        var skill = character.CreateSkill(registrationId, "Arcana", intScore.Id, intScore.Abbreviation);
        var save = character.CreateSavingThrow(registrationId, "Int Save", intScore.Id, intScore.Abbreviation);
        character.ClearDomainEvents();

        // Act - apply -3 (score 7, mod -2)
        character.UpdateAbilityAdditionalScore(intScore.Id, -3);

        // Assert
        intScore.AdditionalScore.Should().Be(-3);
        intScore.CalculatedScore.Should().Be(7);
        intScore.CalculatedModifier.Should().Be(-2);
        skill.AbilityScoreModifier.Should().Be(-2);
        save.AbilityScoreModifier.Should().Be(-2);
    }
}
