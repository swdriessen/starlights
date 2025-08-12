using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain;
using Starlights.Platform.Components.Data.EntityFramework;
using Starlights.Platform.Eventing;

namespace Starlights.Modules.Characters.Data.EntityFramework.EventProcessing;

/// <summary>
/// A simple background service that processes domain events from the database. 
/// The polling is good enough for the current state.
/// </summary>
public sealed class DomainEventProcessingService : BackgroundService
{
    internal const int InitialInterval = 100;
    internal const int MaximumInterval = 10_000;
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
        _logger.LogInformation("starting EventProcessing service... [batch='{BatchSize}', max-interval='{MaximumInterval}ms']", BatchSize, MaximumInterval);

        var interval = TimeSpan.FromMilliseconds(InitialInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _factory.CreateScope();
            {
                var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<CharactersContext>>();

                await using var context = await contextFactory.CreateDbContextAsync(stoppingToken);

                var newEvents = await context.Set<EventMessage>()
                    .Where(x => x.ProcessedOn == null)
                    .OrderBy(x => x.OccurredOn)
                    .Take(BatchSize)
                    .ToListAsync(cancellationToken: stoppingToken);

                if (newEvents.Count > 0)
                {
                    using var _ = CharactersInstrumentation.StartActivity($"Process Event Messages ({newEvents.Count})", a => a.AddTag("newEvents", newEvents.Count));

                    _logger.LogDebug("processing {Count} new domain events", newEvents.Count);

                    var publisher = scope.ServiceProvider.GetRequiredService<IDomainEventPublisher>();

                    foreach (var item in newEvents)
                    {
                        try
                        {
                            var eventType = ResolveEventType(item.EventType);

                            if (eventType == null)
                            {
                                _logger.LogError("Event type '{EventType}' could not be resolved.", item.EventType);
                                continue;
                            }

                            _logger.LogDebug("processing event: {EventType} ({EventId})", eventType.Name, item.Id);

                            var deserialized = JsonSerializer.Deserialize(item.Payload, eventType);
                            if (deserialized is IDomainEvent domainEvent)
                            {
                                _logger.LogInformation("publishing event: {EventType} ({EventId})", eventType.Name, item.Id);
                                await publisher.PublishAsync(domainEvent);

                                // Mark the event as processed
                                item.ProcessedOn = DateTime.UtcNow;
                                await context.SaveChangesAsync(stoppingToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Exception while publishing the message [id='{MessageId}', type='{EventType}']", item.Id, item.EventType);
                            item.ErrorMessage = ex.Message; // stored when the save is moved to bulk save
                            throw;
                        }
                    }
                }

                interval = newEvents.Count switch
                {
                    BatchSize => TimeSpan.FromMilliseconds(0), // if we processed a full batch, reset the interval to 0 to poll immediately
                    > 0 => TimeSpan.FromMilliseconds(InitialInterval),
                    _ => TimeSpan.FromMilliseconds(Math.Min(interval.TotalMilliseconds * 2, MaximumInterval)),
                };
            }

            _logger.LogDebug("service waiting... [interval='{Interval}ms', IsCancellationRequested={IsCancellationRequested}]", interval.TotalMilliseconds, stoppingToken.IsCancellationRequested);
            await Task.Delay(interval, stoppingToken);
        }

        _logger.LogWarning("stopping EventProcessing service...");
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
