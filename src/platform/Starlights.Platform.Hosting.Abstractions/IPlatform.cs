using Microsoft.Extensions.Hosting;

namespace Starlights.Platform.Hosting;

public interface IPlatform
{
    /// <summary>
    /// Gets the application host for the platform.
    /// </summary>
    IHost Host { get; }

    /// <summary>
    /// Gets the options for configuring the platform host.
    /// </summary>
    PlatformHostOptions Options { get; }
}
