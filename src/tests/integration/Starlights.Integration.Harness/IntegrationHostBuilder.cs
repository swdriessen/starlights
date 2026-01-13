using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Integration;

/// <summary>
/// Builder for creating an integration host.
/// </summary>
public class IntegrationHostBuilder
{
    private readonly List<Action<IServiceCollection>> _configureServices = [];
    private readonly List<Action<IntegrationHostOptions>> _configureOptionsCollection = [];

    /// <summary>
    /// Gets a collection of custom properties associated with the current instance.
    /// </summary>
    public Dictionary<string, object> Properties { get; } = [];

    /// <summary>
    /// Adds a configuration action to customize the integration host options.
    /// </summary>
    public IntegrationHostBuilder ConfigureOptions(Action<IntegrationHostOptions> options)
    {
        _configureOptionsCollection.Add(options);
        return this;
    }

    public IntegrationHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
    {
        _configureServices.Add(configureServices);
        return this;
    }

    /// <summary>
    /// Builds the integration host.
    /// </summary>
    public IntegrationHost Build()
    {
        return new IntegrationHost(
            properties =>
            {
                foreach (var (key, value) in Properties)
                {
                    properties[key] = value;
                }
            },
            options =>
            {
                foreach (var configureOptions in _configureOptionsCollection)
                {
                    configureOptions(options);
                }
            },
            services =>
            {
                foreach (var configureServices in _configureServices)
                {
                    configureServices(services);
                }
            }
        );
    }
}
