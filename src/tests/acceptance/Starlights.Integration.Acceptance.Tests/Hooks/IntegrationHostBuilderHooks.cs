using Microsoft.Extensions.DependencyInjection;
using Starlights.Integration.Drivers;
using Starlights.Integration.Drivers.Characters;

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
            .WithTestContext(testContext)
            .WithDriverContext<ElementsDriverContext>()
            .WithDriverContext<CharactersDriverContext>();
    }

    [BeforeScenario]
    public void BuildHost()
    {
        _host = _builder.Build();

        // inject the host and driver context into the scenario container to ease dependency resolution
        _scenarioContext.ScenarioContainer.RegisterInstanceAs<IIntegrationHost>(_host);
        _scenarioContext.ScenarioContainer.RegisterInstanceAs(_host.Services.GetRequiredService<ElementsDriverContext>());
        _scenarioContext.ScenarioContainer.RegisterInstanceAs(_host.Services.GetRequiredService<CharactersDriverContext>());
    }

    [AfterScenario]
    public void DisposeHost()
    {
        _host?.Dispose();
    }
}
