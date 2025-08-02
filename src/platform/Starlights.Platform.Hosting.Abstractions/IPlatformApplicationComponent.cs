using Microsoft.Extensions.Hosting;

namespace Starlights.Platform.Hosting;

public interface IPlatformApplicationComponent
{
    /// <summary>
    /// Gets the registration order of the application configuration component. The lower the number, the earlier the configuration is applied.
    /// </summary>
    int RegistrationOrder { get; }

    /// <summary>
    /// Configures the application host for the platform component.
    /// </summary>
    void UseComponent(IHost host);
}
