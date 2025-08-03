namespace Starlights.Integration.Tests.Core;

/// <summary>
/// Builder for creating an integration host.
/// </summary>
public class IntegrationHostBuilder
{
    private readonly List<Action<IntegrationHostOptions>> _configureActions = [];

    /// <summary>
    /// Configures the integration host to use a console activity processor for OpenTelemetry tracing.
    /// </summary>
    public IntegrationHostBuilder WithConsoleActivityProcessor()
    {
        _configureActions.Add(o => o.UseConsoleActivityProcessor = true);
        return this;
    }

    /// <summary>
    /// Builds the integration host.
    /// </summary>
    public IntegrationHost Build()
    {
        return new IntegrationHost(options =>
        {
            foreach (var configureAction in _configureActions)
            {
                configureAction(options);
            }
        });
    }
}
