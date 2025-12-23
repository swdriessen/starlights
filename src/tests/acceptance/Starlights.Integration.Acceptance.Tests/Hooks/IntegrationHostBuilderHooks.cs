using Starlights.Integration.Drivers.Elements;

namespace Starlights.Integration.Acceptance.Tests.Hooks;

[Binding]
public class IntegrationHostBuilderHooks
{
    public const string ScenarioContextKey = "ScenarioContext";

    private readonly ScenarioContext _scenarioContext;
    private readonly IntegrationHostBuilder _builder;
    private IntegrationHost? _host;

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
        _host = _builder.Build();
        _scenarioContext.ScenarioContainer.RegisterInstanceAs<IIntegrationHost>(_host);
    }

    [AfterScenario]
    public void DisposeHost()
    {
        _host?.Dispose();
    }
}
