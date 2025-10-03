using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Integration.Core.Extensions;

public static class DriverExtensions
{
    /// <summary>
    /// Retrieves a required service of the specified type from the integration host's service provider.
    /// </summary>
    /// <remarks>
    /// Throws an exception if the specified service type is not registered with the service provider.
    /// </remarks>
    public static TDriver GetDriver<TDriver>(this IIntegrationHost host) where TDriver : notnull
    {
        return host.Services.GetRequiredService<TDriver>();
    }
}