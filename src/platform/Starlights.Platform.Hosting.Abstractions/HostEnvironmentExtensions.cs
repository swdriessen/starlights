using Microsoft.Extensions.Hosting;

namespace Starlights.Platform.Hosting;

public static class HostEnvironmentExtensions
{
    /// <summary>
    /// Checks if the current environment is set to "Integration".
    /// </summary>
    public static bool IsIntegration(this IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);
        return environment.IsEnvironment("Integration");
    }
}