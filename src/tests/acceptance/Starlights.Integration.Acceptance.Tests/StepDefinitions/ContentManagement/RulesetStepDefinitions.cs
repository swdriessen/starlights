using Starlights.Integration.Acceptance.Tests.Extensions;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.ContentManagement;

[Binding]
public class RulesetStepDefinitions
{
    private readonly IIntegrationHost _host;

    public RulesetStepDefinitions(IIntegrationHost host)
    {
        _host = host;
    }

    [Given("the core ruleset is initialized")]
    public async Task GivenTheCoreRulesetIsInitializedAsync()
    {
        var client = _host.CreateClient();
        _ = await client.GetAsync("/api/elements/initialize", _host.CancellationToken);
    }

    [Given("I am authenticated as a content creator")]
    public void GivenIAmAuthenticatedAsAContentCreator()
    {
        _host.WriteStepNotImplemented();
    }
}
