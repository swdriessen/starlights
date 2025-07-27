using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Hosting.Abstractions;

namespace Starlights.Platform.Hosting;

public static class HostingExtensions
{
    /// <summary>
    /// Adds the Starlights Platform to the application builder.
    /// </summary>
    public static TBuilder AddStarlightsPlatform<TBuilder>(this TBuilder builder, Action<PlatformBuilderOptions>? optionsAction = null)
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);

        var options = new PlatformBuilderOptions();
        optionsAction?.Invoke(options);

        // register the platform services
        var platformBuilder = new PlatformBuilder(builder.Services, options);
        platformBuilder.Properties["IHostApplicationBuilder"] = builder; // store the original builder for now here, probably need to pass this in the platform builder
        platformBuilder.Build();

        return builder;
    }

    /// <summary>
    /// Initializes the Starlights Platform for the host application.
    /// </summary>
    public static THost UseStarlightsPlatform<THost>(this THost host, Action<PlatformHostOptions>? optionsAction = null)
        where THost : IHost
    {
        ArgumentNullException.ThrowIfNull(host);

        var options = new PlatformHostOptions();
        optionsAction?.Invoke(options);

        var platform = new Platform(host, options);

        // configure the application
        platform.InvokeApplicationExtensions();

        // TODO: initialize modules ?
        foreach (var module in host.Services.GetServices<IPlatformModule>())
        {
            Debug.WriteLine($"Initializing module: {module.GetType().Name}");
        }

        return host;
    }
}
