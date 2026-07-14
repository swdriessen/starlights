using AwesomeAssertions;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Integration.Drivers.Characters.Manage;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.CharacterBuilder;

[Binding]
public class AbilityScoresStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly CharacterManagementDriver _charactersDriver;
    private readonly AbilityScoreDriver _abilitiesDriver;

    public AbilityScoresStepDefinitions(IIntegrationHost host)
    {
        _host = host;
        _charactersDriver = _host.GetDriver<CharacterManagementDriver>();
        _abilitiesDriver = _host.GetDriver<AbilityScoreDriver>();
    }

    [When(@"the user creates a character")]
    public async Task WhenTheUserCreatesACharacterAsync()
    {
        await _charactersDriver.CreateCharacterAsync();
    }

    [Then(@"the character should have the following ability scores:")]
    public async Task ThenTheCharacterShouldHaveTheFollowingAbilityScoresAsync(DataTable dataTable)
    {
        var expectedAbilities = dataTable.CreateSet<AbilityScoreRow>();

        var abilities = await _abilitiesDriver.GetAbilityScores();

        // check that each expected ability score is present in the actual abilities with the correct base score
        foreach (var expected in expectedAbilities)
        {
            var actual = abilities.FirstOrDefault(a => a.Name == expected.Ability);
            actual.Should().NotBeNull($"the character should have an ability score for '{expected.Ability}'");
            actual.BaseScore.Should().Be(expected.BaseScore, $"the character's '{expected.Ability}' ability score should have a base score of {expected.BaseScore}");
        }
    }

    [Given(@"the user creates a character")]
    public async Task GivenTheUserCreatesACharacterAsync()
    {
        await _charactersDriver.CreateCharacterAsync();
    }

    [Then(@"the character should have the ""([^""]*)"" ability score generation method")]
    public async Task ThenTheCharacterShouldHaveTheAbilityScoreGenerationMethodAsync(string p0)
    {
        _host.ThrowStepNotImplemented();
    }

    [When(@"the user chooses the ""([^""]*)"" ability score generation method for the character")]
    public async Task WhenTheUserChoosesTheAbilityScoreGenerationMethodForTheCharacterAsync(string p0)
    {
        _host.ThrowStepNotImplemented();
    }

    [Given(@"the user has a character with the ""([^""]*)"" ability score generation method")]
    public async Task GivenTheUserHasACharacterWithTheAbilityScoreGenerationMethodAsync(string p0)
    {
        _host.ThrowStepNotImplemented();
    }

    [Given(@"the character has the following ability scores:")]
    public async Task GivenTheCharacterHasTheFollowingAbilityScoresAsync(DataTable dataTable)
    {
        var rows = dataTable.CreateSet<AbilityScoreRow>();

        foreach (var row in rows)
        {
            await _abilitiesDriver.UpdateAbilityScoreBase(row.Ability, row.BaseScore);
        }
    }

    [Then(@"the character should have (.*) point buy points available")]
    public async Task ThenTheCharacterShouldHavePointBuyPointsAvailableAsync(int p0)
    {
        _host.ThrowStepNotImplemented();
    }

    [When(@"the user changes the character's ""([^""]*)"" ability score to (.*)")]
    public async Task WhenTheUserChangesTheCharactersAbilityScoreToAsync(string ability, int newScore)
    {
        await _abilitiesDriver.UpdateAbilityScoreBase(ability, newScore);
    }

    [When(@"the user swaps the character's ""([^""]*)"" ability score with the ""([^""]*)"" ability score")]
    public async Task WhenTheUserSwapsTheCharactersAbilityScoreWithTheAbilityScoreAsync(string left, string right)
    {
        var leftScore = await _abilitiesDriver.GetAbilityScore(left);
        var rightScore = await _abilitiesDriver.GetAbilityScore(right);




        _host.ThrowStepNotImplemented();
    }

    [When(@"the user chooses the ""([^""]*)"" ability score generation method for character ""([^""]*)""")]
    public async Task WhenTheUserChoosesTheAbilityScoreGenerationMethodForCharacterAsync(string p0, string bruenor)
    {
        _host.ThrowStepNotImplemented();
    }

    [Then(@"the character ""([^""]*)"" should have the following ability scores:")]
    public async Task ThenTheCharacterShouldHaveTheFollowingAbilityScoresAsync(string bruenor, DataTable dataTable)
    {
        _host.ThrowStepNotImplemented();
    }

    [When(@"the user selects the standard array method for generating the ability scores of the character ""([^""]*)""")]
    public async Task WhenTheUserSelectsTheStandardArrayMethodForGeneratingTheAbilityScoresOfTheCharacterAsync(string p0)
    {
        _host.ThrowStepNotImplemented();
    }

    [Then(@"the character ""([^""]*)"" should have the following base ability scores")]
    public async Task ThenTheCharacterShouldHaveTheFollowingBaseAbilityScoresAsync(string p0, DataTable dataTable)
    {
        _host.ThrowStepNotImplemented();
    }

    #region

    private record AbilityScoreRow
    {
        public string Ability { get; set; }
        public int BaseScore { get; set; }
    }

    #endregion
}
