using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Starlights.Application;

namespace Starlights.Integration;

/// <summary>
/// Integration host for running integration tests against the Starlights application.
/// </summary>
public class IntegrationHost : IIntegrationHost, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IServiceScope _scope;
    private bool _disposedValue;

    internal IntegrationHost(WebApplicationFactory<Program> factory, Dictionary<string, object> properties)
    {
        _factory = factory;
        Properties = properties;
        _scope = _factory.Services.CreateScope();
        Services = _scope.ServiceProvider;
    }

    /// <summary>
    /// Gets a dictionary of properties associated with this host, allowing for storage and retrieval of arbitrary data during integration tests.
    /// </summary>
    public Dictionary<string, object> Properties { get; }

    /// <summary>
    /// Gets the services available in this host, allowing for dependency resolution of application services during integration tests.
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    /// Creates a new HTTP client for making requests to the application, allowing for end-to-end testing of API endpoints and application behavior during integration tests.
    /// </summary>
    /// <returns>A new <see cref="HttpClient"/> instance for making requests to the application.</returns>
    public HttpClient CreateClient()
    {
        return _factory.CreateClient();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _scope.Dispose();
                _factory.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="IntegrationHostBuilder"/> class, which can be used to configure and build an <see cref="IntegrationHost"/> for running integration tests against the Starlights application.
    /// </summary>
    /// <returns>A new <see cref="IntegrationHostBuilder"/> instance for configuring and building an <see cref="IntegrationHost"/>.</returns>
    public static IntegrationHostBuilder CreateBuilder()
    {
        return new();
    }
}
