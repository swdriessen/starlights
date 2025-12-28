namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public class ContentManagementForProficienciesStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageElementsDriver _elementsDriver;

    public ContentManagementForProficienciesStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _elementsDriver = _host.GetDriver<ManageElementsDriver>();
    }

    [When(@"the content creator creates a proficiency with the following properties")]
    public async Task WhenTheContentCreatorCreatesAProficiencyWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Then(@"the proficiency should have at least the following properties")]
    public async Task ThenTheProficiencyShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        throw new PendingStepException();
    }
}
