using Microsoft.Extensions.Hosting;

namespace Starlights.Platform.Hosting.Abstractions;

public interface IPlatformServicesExtension
{
    /// <summary>
    /// Gets the registration order of the service configuration. The lower the number, the earlier the configuration is applied.
    /// </summary>
    /// <remarks>
    /// The microsoft extensions will range between 1000 and 2000. This will be detailed more once these extensions are used in the platform.
    /// This allows for extensions to be configured before or after the default services.
    /// </remarks>
    int RegistrationOrder { get; }

    /// <summary>
    /// Configures the services for the platform module.
    /// </summary>
    void ConfigureServices(IHostApplicationBuilder builder);
}
