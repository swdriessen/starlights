using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Integration.Drivers.CharacterCreation;

namespace Starlights.Acceptance.Tests.StepDefinitions;

[Binding]
public class ProficiencyRulesStepDefinitions
{
    private readonly IIntegrationHost _host = default!;
    private readonly CharacterCreationDriver _characterCreationDriver = default!;
    private readonly CharacterManagementDriver _characterManagementDriver = default!;
    private readonly RegistrationDriver _registrationDriver = default!;

    public ProficiencyRulesStepDefinitions(IIntegrationHost host)
    {
        _host = host;
        _characterCreationDriver = _host.GetDriver<CharacterCreationDriver>();
        _characterManagementDriver = _host.GetDriver<CharacterManagementDriver>();
        _registrationDriver = _host.GetDriver<RegistrationDriver>();
    }

    [Given("the player creates a new character")]
    public async Task GivenThePlayerCreatesANewCharacter()
    {
        await _characterCreationDriver.CreateCharacterAsync();
    }

    [Given("the player selects the {string} class as their starting class")]
    [When("the player selects the {string} class as their starting class")]
    public async Task WhenThePlayerSelectsTheClassAsTheirStartingClass(string className)
    {
        await _registrationDriver.RegisterClass(className);
    }

    [When("the player changes the level of the {string} class to {int}")]
    public async Task WhenThePlayerChangesTheLevelOfTheClassTo(string rogue, int newLevel)
    {
        var targetClass = await _characterManagementDriver.GetClassByName(rogue);
        await _characterManagementDriver.LevelUp(targetClass.CharacterClassId, newLevel);
    }

    [Then("the character's level should be {int}")]
    public async Task ThenTheCharactersLevelShouldBe(int level)
    {
        var id = _host.GetCharacterIdentifier();
        var character = await _characterCreationDriver.GetCharacter(id);
        character.Level.Should().Be(level);
    }

    [Then("the character's proficiency bonus should be {int}")]
    public async Task ThenTheCharactersProficiencyBonusShouldBe(int value)
    {
        var id = _host.GetCharacterIdentifier();
        var character = await _characterCreationDriver.GetCharacter(id);
        character.ProficiencyBonus.Should().Be(value);
    }
}
