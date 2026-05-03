using Microsoft.Extensions.DependencyInjection;
using Starlights.Integration.Eventing;

namespace Starlights.Integration.Extensions;

public static class DomainEventExtensions
{
    public static ElementsEventObserverCollection GetElementsEventObserverCollection(this IIntegrationHost host)
    {
        return host.Services.GetRequiredService<ElementsEventObserverCollection>();
    }
}

public static class EventExtensions
{
    extension(IIntegrationHost host)
    {
        public EventObserverCollection Events => host.Services.GetRequiredService<EventObserverCollection>();
    }
}
