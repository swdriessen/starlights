namespace Starlights.Integration;

public interface IIntegrationHost
{
    /// <summary>
    /// Gets a dictionary of properties associated with this host.
    /// </summary>
    Dictionary<string, object> Properties { get; }

    /// <summary>
    /// Gets the services available in this host.
    /// </summary>
    IServiceProvider Services { get; }

    /// <summary>
    /// Creates a new HTTP client for making requests to the application.
    /// </summary>
    HttpClient CreateClient();
}