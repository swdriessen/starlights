namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public class ContentManagementForSavingThrowsStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageElementsDriver _elementsDriver;

    public ContentManagementForSavingThrowsStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _elementsDriver = _host.GetDriver<ManageElementsDriver>();
    }

    [When(@"a content creator creates a saving throw with the following properties")]
    public async Task WhenAContentCreatorCreatesASavingThrowWithTheFollowingPropertiesAsync(DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Then(@"the saving throw should have at least the following properties")]
    public async Task ThenTheSavingThrowShouldHaveAtLeastTheFollowingPropertiesAsync(DataTable dataTable)
    {
        throw new PendingStepException();
    }
}
