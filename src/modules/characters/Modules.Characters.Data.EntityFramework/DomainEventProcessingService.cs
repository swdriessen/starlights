using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Starlights.Platform.Components.Data.EntityFramework;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Data.EntityFramework;

/// <summary>
/// A simple background service that processes domain events from the database. 
/// The polling is good enough for the current state.
/// </summary>
internal sealed class DomainEventProcessingService : BackgroundService
{
    internal const int MaximumInterval = 10;
    internal const int BatchSize = 100;

    private readonly ILogger<DomainEventProcessingService> _logger;
    private readonly IServiceScopeFactory _factory;

    public DomainEventProcessingService(ILogger<DomainEventProcessingService> logger, IServiceScopeFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken);
        _logger.LogInformation("Domain event processing service started.");

        var interval = TimeSpan.FromSeconds(1);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _factory.CreateScope();
            {
                var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<CharactersContext>>();
                await using var context = await contextFactory.CreateDbContextAsync(stoppingToken);

                var publisher = scope.ServiceProvider.GetRequiredService<IDomainEventPublisher>();
                List<EventMessage> newEvents = await context.Set<EventMessage>()
                    .Where(x => x.ProcessedOn == null)
                    .OrderBy(x => x.OccurredOn)
                    .Take(BatchSize)
                    .ToListAsync(cancellationToken: stoppingToken);

                foreach (var item in newEvents)
                {
                    _logger.LogDebug("Processing domain event: {EventId} ({EventType})", item.Id, item.EventType);

                    var eventType = ResolveEventType(item.EventType);

                    if (eventType == null)
                    {
                        _logger.LogError("Event type '{EventType}' could not be resolved.", item.EventType);
                        continue;
                    }

                    var deserialized = JsonSerializer.Deserialize(item.Payload, eventType);

                    if (deserialized is IDomainEvent domainEvent)
                    {
                        _logger.LogInformation("Publishing domain event: {EventId} ({EventType})", item.Id, item.EventType);
                        await publisher.PublishAsync(domainEvent);

                        // Mark the event as processed
                        item.ProcessedOn = DateTime.UtcNow;
                        await context.SaveChangesAsync(stoppingToken);
                    }
                }

                interval = newEvents.Count switch
                {
                    > 0 => TimeSpan.FromSeconds(1),
                    _ => TimeSpan.FromSeconds(Math.Min(interval.TotalSeconds * 2, MaximumInterval)),
                };
            }

            _logger.LogDebug("Waiting {Delay}s until next processing check... [IsCancellationRequested={IsCancellationRequested}]", interval.TotalSeconds, stoppingToken.IsCancellationRequested);
            await Task.Delay(interval, stoppingToken);
        }

        _logger.LogWarning("Domain event processing service is stopping.");
    }

    private static Type? ResolveEventType(string eventTypeName)
    {
        // Try direct resolution first (works for FullName and exact AssemblyQualifiedName)
        var eventType = Type.GetType(eventTypeName);
        if (eventType != null)
        {
            return eventType;
        }

        // If it's an assembly qualified name, try version-agnostic resolution
        if (eventTypeName.Contains(','))
        {
            // Extract just the type name part (before the first comma)
            var typeNameOnly = eventTypeName.Split(',')[0].Trim();

            // Search all loaded assemblies for this type
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                eventType = assembly.GetType(typeNameOnly);
                if (eventType != null)
                {
                    return eventType;
                }
            }
        }
        else
        {
            // If it's just a type name, search loaded assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                eventType = assembly.GetType(eventTypeName);
                if (eventType != null)
                {
                    return eventType;
                }
            }
        }

        return null;
    }
}
