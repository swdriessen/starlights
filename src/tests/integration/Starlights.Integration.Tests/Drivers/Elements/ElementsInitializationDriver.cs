using Starlights.Integration.Core;

namespace Starlights.Integration.Drivers.Elements;

internal class ElementsInitializationDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsEndpointDriver _endpointDriver;

    public ElementsInitializationDriver(IIntegrationHost integration, ElementsEndpointDriver endpointDriver)
    {
        _integration = integration;
        _endpointDriver = endpointDriver;
    }

    public Task InitializeElementsAsync(CancellationToken cancellation)
    {
        return _endpointDriver.InitializeElementsAsync(cancellation);
    }
}
