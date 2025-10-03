namespace Starlights.Integration.Core;

/// <summary>
/// Builder for creating an integration host.
/// </summary>
public class IntegrationHostBuilder
{
    private readonly List<Action<IntegrationHostOptions>> _configureActions = [];

    public IntegrationHostBuilder()
    {

    }

    /// <summary>
    /// Gets a collection of custom properties associated with the current instance.
    /// </summary>
    public Dictionary<string, object> Properties { get; } = [];

    /// <summary>
    /// Adds a configuration action to customize the integration host options.
    /// </summary>
    public IntegrationHostBuilder ConfigureOptions(Action<IntegrationHostOptions> options)
    {
        _configureActions.Add(options);
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
                // add all properties to the host
                foreach (var (key, value) in Properties)
                {
                    properties[key] = value;
                }

                // rather register an IntegrationTestContext if a TestContext was provided?
            },
            options =>
            {
                foreach (var configureAction in _configureActions)
                {
                    configureAction(options);
                }
            }

        );
    }
}
