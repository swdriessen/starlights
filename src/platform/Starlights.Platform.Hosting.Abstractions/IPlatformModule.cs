using Microsoft.Extensions.Hosting;

namespace Starlights.Platform.Hosting.Abstractions;

/// <summary>
/// Defines a module that can be used to configure services in the platform.
/// </summary>
public interface IPlatformModule
{
    /// <summary>
    /// Configures the services for the platform module.
    /// </summary>
    void ConfigureServices(IHostApplicationBuilder builder);
}
