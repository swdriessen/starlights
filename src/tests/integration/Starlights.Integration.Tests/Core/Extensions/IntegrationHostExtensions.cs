using Microsoft.Extensions.DependencyInjection;
using Starlights.Integration.Tests.Core.Eventing;

namespace Starlights.Integration.Tests.Core.Extensions;

public static class IntegrationHostExtensions
{
    public static IntegrationHostOptions GetIntegrationHostOptions(this IntegrationHost host)
    {
        return host.Properties["IntegrationHostOptions"] as IntegrationHostOptions
            ?? throw new InvalidOperationException("IntegrationHostOptions not found in properties.");
    }

    public static IntegrationEventHandlerListener GetIntegrationEventHandlerListener(this IntegrationHost host)
    {
        return host.Services
            .GetRequiredService<IntegrationEventHandlerListener>();
    }
}
