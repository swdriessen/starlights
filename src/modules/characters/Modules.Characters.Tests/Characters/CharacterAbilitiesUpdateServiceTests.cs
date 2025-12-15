using AwesomeAssertions;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.SavingThrows;
using Starlights.Modules.Characters.Domain.SavingThrows.Eventing;
using Starlights.Modules.Characters.Domain.Services;
using Starlights.Modules.Characters.Domain.Skills;
using Starlights.Modules.Characters.Domain.Skills.Eventing;

namespace Starlights.Modules.Characters.Tests.Characters;

[TestClass]
public sealed class CharacterAbilitiesUpdateServiceTests
{
    private readonly Character _character;
    private readonly AbilitiesComponent _abilitiesComponent;
    private readonly SkillsComponent _skillsComponent;
    private readonly SavingThrowsComponent _savingThrowsComponent;

    private readonly RegistrationId _testRegistrationId;

    // SUT
    private readonly CharacterAbilitiesUpdateService _service;

    public CharacterAbilitiesUpdateServiceTests()
    {
        var creationService = new CharacterCreationService();

        _character = creationService.Create("Test Character");
        _abilitiesComponent = _character.GetRequiredComponent<AbilitiesComponent>();
        _skillsComponent = _character.GetRequiredComponent<SkillsComponent>();
        _savingThrowsComponent = _character.GetRequiredComponent<SavingThrowsComponent>();

        _testRegistrationId = RegistrationId.New();

        _service = new CharacterAbilitiesUpdateService();
    }

    [TestMethod]
    public void UpdateAbilityBaseScore_ShouldUpdateAbilityAndCascadeToSkillAndSavingThrow()
    {
        // Arrange
        var str = _abilitiesComponent.CreateAbilityScore(RegistrationId.New(), "Strength", "STR", 0);
        var skill = _skillsComponent.CreateSkill(RegistrationId.New(), "Athletics", str.Id, str.Abbreviation);
        var save = _savingThrowsComponent.CreateSavingThrow(RegistrationId.New(), "Strength Save", str.Id, str.Abbreviation);
        _character.ClearDomainEvents(); // clear creation events for a clean slate

        // Act
        _service.UpdateAbilityBaseScore(_character, str.Id, 18); // modifier should become +4

        // Assert ability
        str.BaseScore.Should().Be(18);
        str.CalculatedScore.Should().Be(18); // additional remains 0
        str.CalculatedModifier.Should().Be(4);

        // Assert cascading updates
        skill.AbilityScoreModifier.Should().Be(4);
        save.AbilityScoreModifier.Should().Be(4);

        // Domain events: 1 AbilityScoreUpdated + 1 SkillUpdated + 1 SavingThrowUpdated
        _abilitiesComponent.DomainEvents.OfType<AbilityScoreUpdatedEvent>().Should().ContainSingle(e => e.AbilityScoreId == str.Id);
        _skillsComponent.DomainEvents.OfType<SkillUpdatedEvent>().Should().ContainSingle(e => e.SkillId == skill.Id);
        _savingThrowsComponent.DomainEvents.OfType<SavingThrowUpdatedEvent>().Should().ContainSingle(e => e.SavingThrowId == save.Id);
    }

    [TestMethod]
    public void UpdateAbilityBaseScore_ShouldThrow_WhenOutOfRange()
    {
        // Arrange
        var registrationId = RegistrationId.New();
        var str = _abilitiesComponent.CreateAbilityScore(registrationId, "Strength", "STR", 0);

        // Act
        var actLow = () => _service.UpdateAbilityBaseScore(_character, str.Id, 0);
        var actHigh = () => _service.UpdateAbilityBaseScore(_character, str.Id, 21);

        // Assert
        actLow.Should().Throw<ArgumentOutOfRangeException>();
        actHigh.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void UpdateAbilityAdditionalScore_ShouldAdjustCalculatedValuesAndCascade()
    {
        // Arrange
        var registrationId = RegistrationId.New();
        var dex = _abilitiesComponent.CreateAbilityScore(registrationId, "Dexterity", "DEX", 0); // base 10 mod 0
        var skill = _skillsComponent.CreateSkill(registrationId, "Stealth", dex.Id, dex.Abbreviation);
        var save = _savingThrowsComponent.CreateSavingThrow(registrationId, "Dex Save", dex.Id, dex.Abbreviation);
        _character.ClearDomainEvents();

        // Act - add +2 additional (score 12, mod +1)
        _service.UpdateAbilityAdditionalScore(_character, dex.Id, 2);

        // Assert
        dex.AdditionalScore.Should().Be(2);
        dex.CalculatedScore.Should().Be(12);
        dex.CalculatedModifier.Should().Be(1);
        skill.AbilityScoreModifier.Should().Be(1);
        save.AbilityScoreModifier.Should().Be(1);
        _abilitiesComponent.DomainEvents.OfType<AbilityScoreUpdatedEvent>().Should().ContainSingle(e => e.AbilityScoreId == dex.Id);
        _skillsComponent.DomainEvents.OfType<SkillUpdatedEvent>().Should().ContainSingle(e => e.SkillId == skill.Id);
        _savingThrowsComponent.DomainEvents.OfType<SavingThrowUpdatedEvent>().Should().ContainSingle(e => e.SavingThrowId == save.Id);
    }

    [TestMethod]
    public void UpdateAbilityAdditionalScore_CanApplyNegativePenalty()
    {
        // Arrange
        var registrationId = RegistrationId.New();
        var intScore = _abilitiesComponent.CreateAbilityScore(registrationId, "Intelligence", "INT", 0); // base 10 mod 0
        var skill = _skillsComponent.CreateSkill(registrationId, "Arcana", intScore.Id, intScore.Abbreviation);
        var save = _savingThrowsComponent.CreateSavingThrow(registrationId, "Int Save", intScore.Id, intScore.Abbreviation);
        _character.ClearDomainEvents();

        // Act - apply -3 (score 7, mod -2)
        _service.UpdateAbilityAdditionalScore(_character, intScore.Id, -3);

        // Assert
        intScore.AdditionalScore.Should().Be(-3);
        intScore.CalculatedScore.Should().Be(7);
        intScore.CalculatedModifier.Should().Be(-2);
        skill.AbilityScoreModifier.Should().Be(-2);
        save.AbilityScoreModifier.Should().Be(-2);
    }
}
