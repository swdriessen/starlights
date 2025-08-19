using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using Starlights.Application;
using Starlights.Integration.Tests.Core.Eventing;
using Starlights.Platform.Eventing.EventPublisher;

namespace Starlights.Integration.Tests.Core;

/// <summary>
/// Integration host for running integration tests against the Starlights application.
/// </summary>
public class IntegrationHost : IIntegrationHost
{
    public const int Timeout = 5000;
    public const int TimeoutForDebugging = int.MaxValue;

    private readonly WebApplicationFactory<Program> _factory;

    public IntegrationHost(Action<IntegrationHostOptions>? configure = null)
    {
        var options = new IntegrationHostOptions();
        configure?.Invoke(options);

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
                    // able to listen and wait for domain events
                    services.AddSingleton<IntegrationEventHandlerListener>();
                    services.AddDomainEventHandlersFrom(typeof(IntegrationHost).Assembly);

                    if (options.UseConsoleActivityProcessor) // show intrumentation in the console
                    {
                        services.AddOpenTelemetry()
                            .WithTracing(tracing => tracing.AddProcessor<CustomConsoleActivityProcessor>());
                    }
                });
            });

        Properties["IntegrationHostOptions"] = options;

        Services = _factory.Services.CreateScope().ServiceProvider;
    }

    public Dictionary<string, object> Properties { get; } = [];

    public IServiceProvider Services { get; }

    public HttpClient CreateClient() => _factory.CreateClient();

    /// <summary>
    /// Creates a new instance of the <see cref="IntegrationHostBuilder"/>.
    /// </summary>
    public static IntegrationHostBuilder CreateBuilder() => new();
}
