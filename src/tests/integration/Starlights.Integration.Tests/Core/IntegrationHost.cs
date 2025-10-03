using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using Starlights.Application;
using Starlights.Integration.Core.Eventing;
using Starlights.Integration.Drivers.CharacterCreation;
using Starlights.Integration.Drivers.Elements;
using Starlights.Platform.Eventing.EventPublisher;

namespace Starlights.Integration.Core;

/// <summary>
/// Integration host for running integration tests against the Starlights application.
/// </summary>
public class IntegrationHost : IIntegrationHost
{
    public const int Timeout = 5_000;
    public const int TimeoutForDebugging = int.MaxValue;

    private readonly WebApplicationFactory<Program> _factory;

    public IntegrationHost(Action<Dictionary<string, object>>? configureProperties = null, Action<IntegrationHostOptions>? configure = null)
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
                    services.AddSingleton<EventObserverCollection>();
                    services.AddDomainEventHandlersFrom(typeof(IntegrationHost).Assembly);

                    // register drivers
                    // IDriver => auto register
                    services.AddSingleton<CharacterCreationOptionsDriver>();

                    services.AddSingleton<CharacterCreationDriver>();
                    services.AddSingleton<CharacterCreationEndpointDriver>();

                    services.AddSingleton<CharacterManagementDriver>();
                    services.AddSingleton<CharacterManagementEndpointDriver>();

                    services.AddSingleton<AbilityScoresEndpointDriver>();
                    services.AddSingleton<AbilityScoreDriver>();

                    services.AddSingleton<SkillsEndpointDriver>();
                    services.AddSingleton<SkillsDriver>();

                    services.AddSingleton<SavingThrowEndpointDriver>();
                    services.AddSingleton<SavingThrowDriver>();

                    services.AddSingleton<RegistrationEndpointDriver>();
                    services.AddSingleton<RegistrationDriver>();

                    services.AddSingleton<ElementsInitializationDriver>();
                    services.AddSingleton<ElementsEndpointDriver>();

                    // drivers need the instance of this host
                    services.AddSingleton<IIntegrationHost>(this);

                    if (options.UseConsoleActivityProcessor) // show intrumentation in the console
                    {
                        services.AddOpenTelemetry()
                            .WithTracing(tracing => tracing.AddProcessor<CustomConsoleActivityProcessor>());
                    }
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

    /// <summary>
    /// Creates a new instance of the <see cref="IntegrationHostBuilder"/>.
    /// </summary>
    public static IntegrationHostBuilder CreateBuilder()
    {
        return new();
    }
}
