using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
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

    public EventObserverT<T> Event<T>() where T : IDomainEvent
    {
        var observer = _observers.GetOrAdd(typeof(T), _ => CreateObserver<T>());
        if (observer is EventObserverT<T> typedObserver)
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

    private EventObserverT<T> CreateObserver<T>() where T : IDomainEvent
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
            var observed = Event<T>().Events.Count;

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
    /// Clears all recorded invocations for character-related events within the current context.
    /// </summary>
    public void ClearInvocations()
    {
        foreach (var observer in _observers.Values)
        {
            observer.ClearInvocations();
        }
    }
}

internal interface IEventObserver
{
    void ClearInvocations();
}

public class EventObserverT<T> : IEventObserver where T : IDomainEvent
{
    private readonly CancellationToken _cancellationToken;
    private readonly object _gate = new();
    private readonly List<WaitRegistration> _waitRegistrations = [];

    public Mock<IDomainEventHandler<T>> Mock { get; } = new();
    public List<T> Events { get; } = [];

    public EventObserverT(CancellationToken cancellationToken)
    {
        Mock.Setup(m => m.HandleAsync(It.IsAny<T>()))
            .Callback<T>(OnEvent)
            .Returns(Task.CompletedTask);

        _cancellationToken = cancellationToken;
    }

    public void ClearInvocations()
    {
        Mock.Invocations.Clear();
        Events.Clear();
    }

    public Task HandleAsync(T domainEvent)
    {
        return Mock.Object.HandleAsync(domainEvent);
    }

    public Task WaitForEvent(Predicate<T>? predicate = null, int count = 1)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be greater than zero.");
        }

        Trace.WriteLine($"[TRC] waiting for event {typeof(T).Name} .... current invocations: {Mock.Invocations.Count}");
        lock (_gate)
        {
            var matchedExisting = Events.Count(evt => predicate == null || predicate(evt));
            if (matchedExisting >= count)
            {
                return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var registration = new WaitRegistration(predicate, count, matchedExisting, tcs);

            registration.CancellationRegistration = _cancellationToken.Register(static state =>
            {
                var wait = (WaitRegistration)state!;
                wait.TaskSource.TrySetCanceled();
                Trace.WriteLine($"[TRC] waiting for event {typeof(T).Name} was cancelled due to test timeout");
            }, registration);

            tcs.Task.ContinueWith(
                _ => RemoveWaitRegistration(registration),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

            _waitRegistrations.Add(registration);
            return tcs.Task;
        }
    }

    private void OnEvent(T domainEvent)
    {
        lock (_gate)
        {
            Events.Add(domainEvent);

            if (_waitRegistrations.Count == 0)
            {
                return;
            }

            foreach (var wait in _waitRegistrations)
            {
                if (wait.Predicate is null || wait.Predicate(domainEvent))
                {
                    wait.MatchedCount++;
                    if (wait.MatchedCount >= wait.ExpectedCount)
                    {
                        wait.TaskSource.TrySetResult(true);
                    }
                }
            }
        }
    }

    private void RemoveWaitRegistration(WaitRegistration registration)
    {
        lock (_gate)
        {
            _waitRegistrations.Remove(registration);
        }

        registration.CancellationRegistration.Dispose();
    }

    private sealed class WaitRegistration(Predicate<T>? predicate, int expectedCount, int matchedCount, TaskCompletionSource<bool> taskSource)
    {
        public Predicate<T>? Predicate { get; } = predicate;
        public int ExpectedCount { get; } = expectedCount;
        public int MatchedCount { get; set; } = matchedCount;
        public TaskCompletionSource<bool> TaskSource { get; } = taskSource;
        public CancellationTokenRegistration CancellationRegistration { get; set; }
    }
}
