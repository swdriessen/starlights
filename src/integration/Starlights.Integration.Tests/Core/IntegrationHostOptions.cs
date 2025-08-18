namespace Starlights.Integration.Tests.Core;

public class IntegrationHostOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to use the console activity processor.
    /// </summary>
    public bool UseConsoleActivityProcessor { get; set; }

    /// <summary>
    /// Gets or sets a unique identifier for the integration environment to be used for uniqueness of individual tests.
    /// </summary>
    public string UniqueIntegrationIdentifier { get; set; } = Guid.NewGuid().ToString("N")[..12];
}