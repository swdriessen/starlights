using AwesomeAssertions;
using Starlights.Integration.Drivers;
using Starlights.Integration.Drivers.Characters.Manage;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.CharacterBuilder;

[Binding]
public class CharacterCreationStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly CharacterManagementDriver _characters;

    public CharacterCreationStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _characters = _host.GetDriver<CharacterManagementDriver>();
    }

    [Given(@"the ""([^""]*)"" ruleset exists")]
    public async Task GivenTheRulesetExistsAsync(string ruleset)
    {
        // we only have one at the moment, so we can ignore the ruleset name for now
        ruleset.Should().NotBeNullOrWhiteSpace("a valid ruleset name must be provided");

        var driver = _host.GetDriver<ElementsInitializationDriver>();
        await driver.InitializeElementsAsync();
    }

    [Given(@"I am authenticated as a user")]
    public static Task GivenIAmAuthenticatedAsAUserAsync()
    {
        // this step is not implemented currently
        return Task.CompletedTask;
    }

    [Given(@"the user's character collection is empty")]
    public async Task GivenTheUsersCharacterCollectionIsEmptyAsync()
    {
        var characters = await _characters.GetCharacters();
        characters.Should().BeEmpty("the user's character collection should be empty");
    }

    [When(@"the user creates a character with the name ""([^""]*)""")]
    public async Task WhenTheUserCreatesACharacterWithTheNameAsync(string name)
    {
        await _characters.CreateCharacterAsync(name);
    }

    [Then(@"the user's character collection should include the character ""([^""]*)""")]
    public async Task ThenTheUsersCharacterCollectionShouldIncludeTheCharacterAsync(string name)
    {
        var characters = await _characters.GetCharacters();
        characters.Should().Contain(c => c.Name == name, $"the user's character collection should include the character '{name}'");
    }

    [Given(@"the user's character collection includes a character named ""([^""]*)""")]
    public async Task GivenTheUsersCharacterCollectionIncludesACharacterNamedAsync(string name)
    {
        var characters = await _characters.GetCharacters();
        if (!characters.Any(c => c.Name == name))
        {
            await _characters.CreateCharacterAsync(name);
        }
    }

    [Then(@"the user's character collection should include two characters named ""([^""]*)""")]
    public async Task ThenTheUsersCharacterCollectionShouldIncludeTwoCharactersNamedAsync(string name)
    {
        var characters = await _characters.GetCharacters();
        characters.Count(c => c.Name == name).Should().Be(2, $"the user's character collection should include two characters named '{name}'");
    }

    [Then(@"the character ""([^""]*)"" should have a portrait")]
    public async Task ThenTheCharacterShouldHaveAPortraitAsync(string name)
    {
        var characters = await _characters.GetCharacters();
        var character = characters.FirstOrDefault(c => c.Name == name);
        character.Should().NotBeNull($"the character '{name}' should exist");
        character.PortraitUrl.Should().NotBeNull($"the character '{name}' should have a portrait");
    }

    [Then(@"the character ""([^""]*)"" should be owned by the user")]
    public async Task ThenTheCharacterShouldBeOwnedByTheUserAsync(string name)
    {
        throw new PendingStepException();
    }

    [Given(@"the user's character collection includes the following characters:")]
    public async Task GivenTheUsersCharacterCollectionIncludesTheFollowingCharactersAsync(DataTable dataTable)
    {
        // table with a single column "Name"
        var names = dataTable.Rows.Select(r => r["name"]).Where(n => !string.IsNullOrWhiteSpace(n)).ToArray();

        foreach (var name in names)
        {
            _host.IntegrationContext.WriteLine(name);
            await _characters.CreateCharacterAsync(name);
        }
    }

    [When(@"the user deletes the character with the name ""([^""]*)""")]
    public async Task WhenTheUserDeletesTheCharacterWithTheNameAsync(string name)
    {
        var characters = await _characters.GetCharacters();
        var character = characters.FirstOrDefault(c => c.Name == name);
        character.Should().NotBeNull($"the character '{name}' should exist in the user's collection to be deleted");
        await _characters.DeleteCharacter(character.CharacterId);
    }

    [Then(@"the user's character collection should only include:")]
    public async Task ThenTheUsersCharacterCollectionShouldOnlyIncludeAsync(DataTable dataTable)
    {
        var expectedNames = dataTable.Rows.Select(r => r["name"]).Where(n => !string.IsNullOrWhiteSpace(n)).ToArray();
        var characters = await _characters.GetCharacters();
        var actualNames = characters.Select(c => c.Name).ToArray();
        actualNames.Should().Equal(expectedNames, "the user's character collection should only include the specified characters");
    }
}
