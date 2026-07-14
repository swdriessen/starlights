using Starlights.Integration.Drivers;
using Starlights.Integration.Extensions;

namespace Starlights.Integration;

public static class IntegrationHostExtensions
{
    public static IntegrationHostOptions GetIntegrationHostOptions(this IIntegrationHost host)
    {
        return host.Properties["IntegrationHostOptions"] as IntegrationHostOptions
            ?? throw new InvalidOperationException("IntegrationHostOptions not found in properties.");
    }

    public static Task InitializeElements(this IIntegrationHost host)
    {
        return host.GetDriver<ElementsInitializationDriver>().InitializeElementsAsync();
    }
}
