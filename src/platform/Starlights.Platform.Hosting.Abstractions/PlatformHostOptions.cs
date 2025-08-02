using System.Reflection;

namespace Starlights.Platform.Hosting;

public class PlatformHostOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to enable discovery of platform application extensions. (default: true)
    /// </summary>
    public bool IsDiscoveryEnabled { get; set; } = true;

    /// <summary>
    /// Gets the list of additional assemblies to be loaded for the platform.
    /// </summary>
    public List<Assembly> AdditionalAssemblies { get; } = [];
}