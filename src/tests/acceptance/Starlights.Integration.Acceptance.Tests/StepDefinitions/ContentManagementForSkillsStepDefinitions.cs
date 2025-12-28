namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public class ContentManagementForSkillsStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageElementsDriver _elementsDriver;

    public ContentManagementForSkillsStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _elementsDriver = _host.GetDriver<ManageElementsDriver>();
    }

    [When(@"the content creator creates a skill with the following properties")]
    public async Task WhenTheContentCreatorCreatesASkillWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Then(@"the skill should have at least the following properties")]
    public async Task ThenTheSkillShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        throw new PendingStepException();
    }
}
