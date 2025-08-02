using Microsoft.Extensions.Hosting;

namespace Starlights.Platform.Hosting;

public interface IPlatformServiceComponent
{
    /// <summary>
    /// Gets the registration order of the service component. The lower the number, the earlier the configuration is applied.
    /// </summary>
    int RegistrationOrder { get; }

    /// <summary>
    /// Configures the services for the platform component.
    /// </summary>
    void ConfigureServices(IHostApplicationBuilder builder);
}
