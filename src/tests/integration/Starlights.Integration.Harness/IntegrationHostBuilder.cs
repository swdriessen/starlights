using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using Starlights.Application;
using Starlights.Integration.Eventing;
using Starlights.Integration.Extensions;
using Starlights.Platform.Eventing.EventPublisher;

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
    /// Adds a driver context to the integration host by registering the assembly containing the specified type and adding the type itself as a singleton service.
    /// </summary>
    public IntegrationHostBuilder RegisterDriverContext<T>() where T : class
    {
        return this.WithDriverAssemblies(typeof(T).Assembly)
            .ConfigureServices(services => services.AddSingleton<T>());
    }

    /// <summary>
    /// Builds the integration host.
    /// </summary>
    public IntegrationHost Build()
    {
        var options = new IntegrationHostOptions();

        foreach (var configureOptions in _configureOptionsCollection)
        {
            configureOptions(options);
        }

        var properties = new Dictionary<string, object>();

        foreach (var (key, value) in Properties)
        {
            properties[key] = value;
        }

        var hostAccessor = new IntegrationHostAccessor();

        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // allow separation from Development using appsettings.Integration.json
                builder.UseEnvironment("Integration");

                // custom integration environment configuration
                builder.UseSetting("INTEGRATION_ENVIRONMENT_UNIQUE_NAME", options.UniqueIntegrationIdentifier);

                // preserve the execution context to ensure that the test server can handle async operations correctly
                builder.UseTestServer(testServerOptions => testServerOptions.PreserveExecutionContext = true);

                // configure additional services for integration tests
                builder.ConfigureTestServices(services =>
                {
                    services.Configure<HostOptions>(hostOptions => hostOptions.ShutdownTimeout = TimeSpan.FromMilliseconds(250));

                    services.AddSingleton(options);

                    // able to listen and wait for domain events
                    services.AddSingleton<EventObserverCollection>();
                    services.AddDomainEventHandlersFrom(options.EventHandlerAssemblies);

                    // auto register all IDriver implementations
                    services.RegisterDrivers(options.DriverAssemblies);

                    services.AddSingleton(_ => hostAccessor.Host ?? throw new InvalidOperationException("IntegrationHost is not initialized."));

                    if (options.UseConsoleActivityProcessor) // show instrumentation in the console logging
                    {
                        services.AddOpenTelemetry()
                            .WithTracing(tracing => tracing.AddProcessor<CustomConsoleActivityProcessor>());
                    }

                    foreach (var configureServices in _configureServices)
                    {
                        configureServices(services);
                    }
                });
            });

        var host = new IntegrationHost(factory, properties);
        hostAccessor.Host = host;

        return host;
    }

    private sealed class IntegrationHostAccessor
    {
        public IIntegrationHost? Host { get; set; }
    }
}
