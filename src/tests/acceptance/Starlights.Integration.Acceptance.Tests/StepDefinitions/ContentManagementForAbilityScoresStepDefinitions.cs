namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public class ContentManagementForAbilityScoresStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageElementsDriver _elementsDriver;

    public ContentManagementForAbilityScoresStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _elementsDriver = _host.GetDriver<ManageElementsDriver>();
    }

    [When(@"a content creator creates an ability score with the following properties")]
    public async Task WhenAContentCreatorCreatesAnAbilityScoreWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Then(@"the ability score should have at least the following properties")]
    public async Task ThenTheAbilityScoreShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Given(@"an ability score exists with the name ""([^""]*)"" and an abbreviation ""([^""]*)""")]
    public async Task GivenAnAbilityScoreExistsWithTheNameAndAnAbbreviationAsync(string intelligence, string iNT)
    {
        throw new PendingStepException();
    }

    [When(@"the content creator updates the ability score ""([^""]*)"" with the following properties")]
    public async Task WhenTheContentCreatorUpdatesTheAbilityScoreWithTheFollowingPropertiesAsync(string intelligence, DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Then(@"the ability score ""([^""]*)"" should have at least the following properties")]
    public async Task ThenTheAbilityScoreShouldHaveAtLeastTheFollowingPropertiesAsync(string intelligence, DataTable dataTable)
    {
        throw new PendingStepException();
    }

}
