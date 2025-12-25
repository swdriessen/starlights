using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Hosting;

namespace Starlights.Modules.Elements.Data.EntityFramework.EventProcessing;

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
    public static void AddElementsEventProcessingComponent(this PlatformBuilderOptions options)
    {
        options.AdditionalAssemblies.Add(typeof(EventProcessingComponent).Assembly);
    }
}
