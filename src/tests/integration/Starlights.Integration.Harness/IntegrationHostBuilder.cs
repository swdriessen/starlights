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
        return this.WithDriverAssembly(typeof(T).Assembly)
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

        var optionsSnapshot = new IntegrationHostOptions
        {
            TestTimeout = options.TestTimeout,
            UseConsoleActivityProcessor = options.UseConsoleActivityProcessor,
            UniqueIntegrationIdentifier = options.UniqueIntegrationIdentifier,
            DriverAssemblies = options.DriverAssemblies
        };

        var hostAccessor = new IntegrationHostAccessor();

        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // allow separation from Development using appsettings.Integration.json
                builder.UseEnvironment("Integration");

                // custom integration environment configuration
                builder.UseSetting("INTEGRATION_ENVIRONMENT_UNIQUE_NAME", optionsSnapshot.UniqueIntegrationIdentifier);

                // preserve the execution context to ensure that the test server can handle async operations correctly
                builder.UseTestServer(testServerOptions => testServerOptions.PreserveExecutionContext = true);

                // configure additional services for integration tests
                builder.ConfigureTestServices(services =>
                {
                    services.Configure<HostOptions>(hostOptions => hostOptions.ShutdownTimeout = TimeSpan.FromMilliseconds(250));

                    services.AddSingleton(optionsSnapshot);

                    // able to listen and wait for domain events
                    services.AddSingleton<EventObserverCollection>();
                    services.AddDomainEventHandlersFrom(typeof(IntegrationHost).Assembly);

                    // auto register all IDriver implementations
                    services.RegisterDrivers(optionsSnapshot.DriverAssemblies ?? [typeof(IntegrationHost).Assembly]);

                    // integration test context to manage cancellation and test-specific data
                    //services.AddSingleton(new IntegrationTestContext(testContext, optionsSnapshot.TestTimeout));

                    services.AddSingleton(_ => hostAccessor.Host ?? throw new InvalidOperationException("IntegrationHost is not initialized."));

                    if (optionsSnapshot.UseConsoleActivityProcessor) // show instrumentation in the console logging
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
