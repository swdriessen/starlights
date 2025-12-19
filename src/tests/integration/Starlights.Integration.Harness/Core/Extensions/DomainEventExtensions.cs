using Microsoft.Extensions.DependencyInjection;
using Starlights.Integration.Core.Eventing;

namespace Starlights.Integration.Core.Extensions;

public static class DomainEventExtensions
{
    public static EventObserverCollection GetEventObserverCollection(this IIntegrationHost host)
    {
        return host.Services.GetRequiredService<EventObserverCollection>();
    }
}
