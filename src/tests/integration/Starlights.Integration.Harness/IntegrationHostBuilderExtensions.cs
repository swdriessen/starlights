using System.Reflection;

namespace Starlights.Integration;

public static class IntegrationHostBuilderExtensions
{
    public static IntegrationHostBuilder WithDrivers<T>(this IntegrationHostBuilder builder)
    {
        var assemblies = new List<Assembly>();

        if (builder.Properties.TryGetValue("driver", out var storedAssemblies))
        {
            assemblies = (List<Assembly>)storedAssemblies;
        }

        var type = typeof(T);
        assemblies.Add(type.Assembly);

        builder.Properties["driver"] = assemblies;

        builder.ConfigureOptions(o => o.DriverAssemblies = [.. assemblies]);
        return builder;
    }

    public static IntegrationHostBuilder WithDriverAssemblies(this IntegrationHostBuilder builder, params Assembly[] assemblies)
    {
        builder.ConfigureOptions(o => o.DriverAssemblies = assemblies);
        return builder;
    }

    public static IntegrationHostBuilder WithDriverAssembly(this IntegrationHostBuilder builder, Assembly assembly)
    {
        builder.ConfigureOptions(o =>
        {
            if (o.DriverAssemblies == null)
            {
                o.DriverAssemblies = new[] { assembly };
            }
            else
            {
                o.DriverAssemblies = o.DriverAssemblies.Append(assembly).ToArray();
            }
        });
        return builder;
    }

    /// <summary>
    /// Configures the integration host to use a console activity processor for OpenTelemetry tracing.
    /// </summary>
    public static IntegrationHostBuilder WithConsoleActivityProcessor(this IntegrationHostBuilder builder)
    {
        builder.ConfigureOptions(o => o.UseConsoleActivityProcessor = true);
        return builder;
    }
}
