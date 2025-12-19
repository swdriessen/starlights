using Microsoft.Extensions.DependencyInjection;
using Starlights.Integration.Eventing;

namespace Starlights.Integration.Extensions;

public static class DomainEventExtensions
{
    public static EventObserverCollection GetEventObserverCollection(this IIntegrationHost host)
    {
        return host.Services.GetRequiredService<EventObserverCollection>();
    }
}
