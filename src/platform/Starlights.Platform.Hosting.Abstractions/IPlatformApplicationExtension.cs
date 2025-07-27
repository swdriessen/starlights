using Microsoft.Extensions.Hosting;

namespace Starlights.Platform.Hosting.Abstractions;

public interface IPlatformApplicationExtension
{
    /// <summary>
    /// Gets the registration order of the application configuration. The lower the number, the earlier the configuration is applied.
    /// </summary>
    /// <remarks>
    /// The microsoft extensions will range between 1000 and 2000. This will be detailed more once these extensions are used in the platform.
    /// This allows for extensions to be configured before, after, or in between the default services.
    /// </remarks>
    int RegistrationOrder => 1000;

    /// <summary>
    /// Configures the application host for the platform module.
    /// </summary>
    void UseExtension(IHost host);
}
