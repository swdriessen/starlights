using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Starlights.Integration.Extensions;
using Starlights.Platform.Eventing;

namespace Starlights.Integration.Eventing;

public sealed class EventObserverCollection
{
    private readonly ILogger<EventObserverCollection> _logger;
    private readonly CancellationToken _cancellationToken;
    private readonly ConcurrentDictionary<Type, IEventObserver> _observers = new();

    public EventObserverCollection(ILogger<EventObserverCollection> logger, IIntegrationHost integration)
    {
        _logger = logger;
        _cancellationToken = integration.CancellationToken;
    }

    private EventObserver<T> Event<T>() where T : IDomainEvent
    {
        var observer = _observers.GetOrAdd(typeof(T), _ => CreateObserver<T>());
        if (observer is EventObserver<T> typedObserver)
        {
            return typedObserver;
        }

        throw new InvalidOperationException($"Observer type mismatch for event type {typeof(T).FullName}.");
    }

    public Task HandleAsync<T>(T domainEvent) where T : IDomainEvent
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        return Event<T>().HandleAsync(domainEvent);
    }

    private EventObserver<T> CreateObserver<T>() where T : IDomainEvent
    {
        return new(_cancellationToken);
    }

    /// <summary>
    /// Ensures that a specified number of domain events of type T matching the given predicate are observed. Otherwise the test fails with a timeout.
    /// </summary>
    public async Task EnsureObservation<T>(Predicate<T>? predicate = null, int count = 1) where T : IDomainEvent
    {
        _logger.LogInformation("⏳ Waiting for {EventType} event{Count}...", typeof(T).Name, count > 1 ? $" x{count}" : string.Empty);

        try
        {
            var e = Event<T>();
            await e.WaitForEvent(predicate, count);
        }
        catch (TaskCanceledException)
        {
            var observed = Event<T>().MatchedCount(predicate);

            var message = observed == 0
                ? $"No {typeof(T).Name} events were observed within the test timeout."
                : $"Only {observed} of the expected {count} {typeof(T).Name} events were observed within the test timeout.";

            _logger.LogError("{Message}", message);
            throw new TimeoutException(message);
        }
        _logger.LogInformation("Observed {EventType} event{Count}", typeof(T).Name, count > 1 ? $" x{count}" : string.Empty);
    }

    /// <summary>
    /// Clears all recorded invocations and events for the specified domain event type.
    /// </summary>
    public void ClearInvocations<T>() where T : IDomainEvent
    {
        Event<T>().ClearInvocations();
    }

    /// <summary>
    /// Clears all recorded invocations for all observed event types within the current context.
    /// </summary>
    public void ClearInvocations()
    {
        foreach (var observer in _observers.Values)
        {
            observer.ClearInvocations();
        }
    }
}
