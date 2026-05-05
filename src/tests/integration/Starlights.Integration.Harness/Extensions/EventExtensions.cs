using Microsoft.Extensions.DependencyInjection;
using Starlights.Integration.Eventing;

namespace Starlights.Integration.Extensions;

public static class EventExtensions
{
    extension(IIntegrationHost host)
    {
        /// <summary>
        /// Retrieves the <see cref="EventObserverCollection"/> from the integration host's service provider, allowing for access to registered event observers and facilitating event-driven testing scenarios during integration tests.
        /// </summary>
        public EventObserverCollection Events => host.Services.GetRequiredService<EventObserverCollection>();
    }
}
