using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using Starlights.Application;
using Starlights.Integration.Eventing;
using Starlights.Integration.Extensions;
using Starlights.Platform.Eventing.EventPublisher;

namespace Starlights.Integration;

/// <summary>
/// Integration host for running integration tests against the Starlights application.
/// </summary>
public class IntegrationHost : IIntegrationHost, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private bool _disposedValue;

    public IntegrationHost(
        Action<Dictionary<string, object>>? configureProperties = null,
        Action<IntegrationHostOptions>? configureOptions = null,
        Action<IServiceCollection>? configureServices = null)
    {
        var options = new IntegrationHostOptions();
        configureOptions?.Invoke(options);

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // allow separation from Development using appsettings.Integration.json
                builder.UseEnvironment("Integration");

                // custom integration environment configuration
                builder.UseSetting("INTEGRATION_ENVIRONMENT_UNIQUE_NAME", options.UniqueIntegrationIdentifier);

                // preserve the execution context to ensure that the test server can handle async operations correctly
                builder.UseTestServer(options => options.PreserveExecutionContext = true);

                // configure additional services for integration tests
                builder.ConfigureTestServices(services =>
                {
                    services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMilliseconds(250));

                    // able to listen and wait for domain events
                    services.AddSingleton<EventObserverCollection>();
                    services.AddDomainEventHandlersFrom(typeof(IntegrationHost).Assembly);

                    services.AddSingleton<ElementsEventObserverCollection>();



                    // auto register all IDriver implementations
                    services.RegisterDrivers(options.DriverAssemblies ?? [typeof(IntegrationHost).Assembly]);

                    // drivers need the instance of this host
                    services.AddSingleton<IIntegrationHost>(this);

                    if (options.UseConsoleActivityProcessor) // show instrumentation in the console logging
                    {
                        services.AddOpenTelemetry()
                            .WithTracing(tracing => tracing.AddProcessor<CustomConsoleActivityProcessor>());
                    }

                    configureServices?.Invoke(services);
                });
            });

        configureProperties?.Invoke(Properties);

        Properties["IntegrationHostOptions"] = options;

        Services = _factory.Services.CreateScope().ServiceProvider;
    }

    public Dictionary<string, object> Properties { get; } = [];

    public IServiceProvider Services { get; }

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
    /// Creates a new instance of the <see cref="IntegrationHostBuilder"/>.
    /// </summary>
    public static IntegrationHostBuilder CreateBuilder()
    {
        return new();
    }
}
