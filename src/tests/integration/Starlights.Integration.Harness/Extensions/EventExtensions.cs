using Microsoft.Extensions.DependencyInjection;
using Starlights.Integration.Eventing;

namespace Starlights.Integration.Extensions;

public static class EventExtensions
{
    extension(IIntegrationHost host)
    {
        public EventObserverCollection Events => host.Services.GetRequiredService<EventObserverCollection>();
    }
}
