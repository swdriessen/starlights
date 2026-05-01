using Starlights.Integration.Extensions;

namespace Starlights.Integration.Drivers;

public class ElementsInitializationDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsEndpointDriver _endpointDriver;

    public ElementsInitializationDriver(IIntegrationHost integration, ElementsEndpointDriver endpointDriver)
    {
        _integration = integration;
        _endpointDriver = endpointDriver;
    }

    public Task InitializeElementsAsync()
    {
        return _endpointDriver.InitializeElementsAsync(_integration.CancellationToken);
    }
}
