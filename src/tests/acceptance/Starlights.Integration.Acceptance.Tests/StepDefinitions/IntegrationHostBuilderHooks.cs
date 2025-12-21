using Starlights.Integration.Drivers.Elements;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public class IntegrationHostBuilderHooks
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IntegrationHostBuilder _builder;

    public IntegrationHostBuilderHooks(TestContext testContext, ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;

        _builder = IntegrationHost.CreateBuilder()
            .WithDriverAssemblies(typeof(ManageSpellsDriver).Assembly)
            .WithTestContext(testContext);
    }

    [BeforeScenario]
    public void BuildHost()
    {
        var host = _builder.Build();
        _scenarioContext.ScenarioContainer.RegisterInstanceAs<IIntegrationHost>(host);
    }

    [AfterScenario]
    public void DisposeHost()
    {
        var host = _scenarioContext.ScenarioContainer.Resolve<IIntegrationHost>();
        (host as IntegrationHost)?.Dispose();
    }
}
