using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Integration.Extensions;

public static class DriverExtensions
{
    /// <summary>
    /// Retrieves a required service of the specified type from the integration host's service provider.
    /// </summary>
    public static TDriver GetDriver<TDriver>(this IIntegrationHost host) where TDriver : notnull
    {
        return host.Services.GetRequiredService<TDriver>();
    }

    /// <summary>
    /// Registers all implementations of the <see cref="IDriver"/> interface from the specified assemblies as singleton services.
    /// </summary>
    public static void RegisterDrivers(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);

        assemblies = assemblies is { Length: > 0 }
            ? assemblies
            : throw new ArgumentException("At least one assembly must be provided.", nameof(assemblies));

        var driverInterface = typeof(IDriver);

        foreach (var driverType in assemblies
                     .SelectMany(a => a.DefinedTypes)
                     .Where(t => !t.IsAbstract && !t.IsInterface && driverInterface.IsAssignableFrom(t))
                     .Select(t => t.AsType()))
        {
            services.AddSingleton(driverType);
        }
    }

    /// <summary>
    /// Registers all implementations of the <see cref="IDriver"/> interface from the calling assembly as singleton services.
    /// </summary>
    public static void RegisterDrivers(this IServiceCollection services)
    {
        services.RegisterDrivers(Assembly.GetCallingAssembly());
    }
}
