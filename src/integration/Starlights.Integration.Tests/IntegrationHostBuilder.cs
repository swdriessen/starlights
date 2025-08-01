namespace Starlights.Integration.Tests;

/// <summary>
/// Builder for creating an integration host.
/// </summary>
public class IntegrationHostBuilder
{
    /// <summary>
    /// Builds the integration host.
    /// </summary>
    public IntegrationHost Build()
    {
        return new IntegrationHost();
    }
}
