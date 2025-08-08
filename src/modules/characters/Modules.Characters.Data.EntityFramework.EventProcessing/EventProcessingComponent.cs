using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Modules.Characters.Data.EntityFramework.EventProcessing;
using Starlights.Platform.Hosting;

namespace Modules.Characters.Data.EntityFramework.EventProcessing;

/// <summary>
/// The platform component for the EventProcessing component that configures the domain event processing service.
/// </summary>
internal class EventProcessingComponent : IPlatformServiceComponent
{
    public int RegistrationOrder => 1030;

    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddHostedService<DomainEventProcessingService>();
    }
}

public static class EventProcessingComponentExtensions
{
    /// <summary>
    /// Adds the EventProcessing component to the platform builder options.
    /// </summary>
    public static void AddEventProcessingComponent(this PlatformBuilderOptions options)
    {
        options.AdditionalAssemblies.Add(typeof(EventProcessingComponent).Assembly);
    }
}
