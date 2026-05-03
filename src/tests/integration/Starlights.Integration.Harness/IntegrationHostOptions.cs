using System.Reflection;

namespace Starlights.Integration;

public class IntegrationHostOptions
{
    /// <summary>
    /// Gets or sets the timeout duration for integration tests. This timeout is used to ensure that tests do not run indefinitely and provides a default value of 10 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets a value indicating whether to use the console activity processor.
    /// </summary>
    public bool UseConsoleActivityProcessor { get; set; }

    /// <summary>
    /// Gets or sets a unique identifier for the integration environment to be used for uniqueness of individual tests.
    /// </summary>
    public string UniqueIntegrationIdentifier { get; set; } = Guid.NewGuid().ToString("N")[..12];

    /// <summary>
    /// Gets or sets the assemblies to scan for <see cref="IDriver"/> implementations.
    /// </summary>
    public Assembly[]? DriverAssemblies { get; set; }
}