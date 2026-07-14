using Starlights.Integration.Acceptance.Tests.Extensions;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.ContentManagement;

[Binding]
public class RulesetStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly IntegrationTestContext _context;

    public RulesetStepDefinitions(IIntegrationHost host, IntegrationTestContext context)
    {
        _host = host;
        _context = context;
    }

    [Given("the core ruleset is initialized")]
    public async Task GivenTheCoreRulesetIsInitializedAsync()
    {
        using var client = _host.CreateClient();
        _ = await client.GetAsync("/api/elements/initialize", _context.CancellationToken);
    }

    [Given("I am authenticated as a content creator")]
    public void GivenIAmAuthenticatedAsAContentCreator()
    {
        _host.WriteStepNotImplemented();
    }
}
