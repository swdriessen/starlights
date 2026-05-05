using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Integration.Extensions;

public static class DriverExtensions
{
    extension(IIntegrationHost host)
    {
        /// <summary>
        /// Retrieves a required service of the specified type from the integration host's service provider.
        /// </summary>
        public TDriver GetDriver<TDriver>() where TDriver : notnull
        {
            return host.Services.GetRequiredService<TDriver>();
        }
    }

    extension(IntegrationHostBuilder builder)
    {
        /// <summary>
        /// Adds the specified assemblies to the list of driver assemblies in the integration host options, allowing the host to discover and register drivers from those assemblies.
        /// </summary>
        public IntegrationHostBuilder WithDriverAssemblies(params Assembly[] assemblies)
        {
            return builder.ConfigureOptions(o => o.DriverAssemblies = [.. o.DriverAssemblies.Concat(assemblies).Distinct()]);
        }
    }

    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers all implementations of the <see cref="IDriver"/> interface from the specified assemblies as singleton services.
        /// </summary>
        internal void RegisterDrivers(params Assembly[] assemblies)
        {
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
        public void RegisterDrivers()
        {
            services.RegisterDrivers(Assembly.GetCallingAssembly());
        }
    }
}
