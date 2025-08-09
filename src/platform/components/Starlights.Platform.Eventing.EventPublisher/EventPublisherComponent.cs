using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Hosting;

namespace Starlights.Platform.Eventing.EventPublisher;

/// <summary>
/// The component for the Starlights Platform that configures the event publisher services.
/// </summary>
public class EventPublisherComponent : IPlatformServiceComponent
{
    public int RegistrationOrder => 500;

    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
    }
}
