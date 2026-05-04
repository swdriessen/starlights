using System.Diagnostics;
using Starlights.Platform.Eventing;

namespace Starlights.Integration.Eventing;

internal sealed class EventObserver<T> : IEventObserver where T : IDomainEvent
{
    private readonly List<T> _events = [];
    private readonly List<WaitRegistration> _waitRegistrations = [];
    private readonly Lock _gate = new();
    private readonly CancellationToken _cancellationToken;

    public EventObserver(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
    }

    public void ClearInvocations()
    {
        lock (_gate)
        {
            _events.Clear();
        }
    }

    public Task HandleAsync(T domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        OnEvent(domainEvent);
        return Task.CompletedTask;
    }

    public int MatchedCount(Predicate<T>? predicate = null)
    {
        lock (_gate)
        {
            return _events.Count(evt => predicate is null || predicate(evt));
        }
    }

    public Task WaitForEvent(Predicate<T>? predicate = null, int count = 1)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be greater than zero.");
        }

        lock (_gate)
        {
            Trace.WriteLine($"[TRC] waiting for event {typeof(T).Name} .... current invocations: {_events.Count}");
            var matchedExisting = _events.Count(evt => predicate == null || predicate(evt));
            if (matchedExisting >= count)
            {
                return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var registration = new WaitRegistration(predicate, count, matchedExisting, tcs);

            registration.CancellationRegistration = _cancellationToken.Register(static state =>
            {
                var context = (CancellationContext)state!;
                context.Observer.CancelWaitRegistration(context.Registration);
            }, new CancellationContext(this, registration));

            _waitRegistrations.Add(registration);
            return tcs.Task;
        }
    }

    private void OnEvent(T domainEvent)
    {
        lock (_gate)
        {
            _events.Add(domainEvent);

            if (_waitRegistrations.Count == 0)
            {
                return;
            }

            List<WaitRegistration>? completedRegistrations = null;

            foreach (var wait in _waitRegistrations)
            {
                if (wait.Predicate is null || wait.Predicate(domainEvent))
                {
                    wait.MatchedCount++;
                    if (wait.MatchedCount >= wait.ExpectedCount && wait.TaskSource.TrySetResult(true))
                    {
                        completedRegistrations ??= [];
                        completedRegistrations.Add(wait);
                    }
                }
            }

            if (completedRegistrations is not null)
            {
                foreach (var registration in completedRegistrations)
                {
                    RemoveWaitRegistration(registration);
                }
            }
        }
    }

    private void CancelWaitRegistration(WaitRegistration registration)
    {
        lock (_gate)
        {
            if (_waitRegistrations.Remove(registration))
            {
                registration.TaskSource.TrySetCanceled(_cancellationToken);
                Trace.WriteLine($"[TRC] waiting for event {typeof(T).Name} was cancelled due to test timeout");
            }
        }

        registration.CancellationRegistration.Dispose();
    }

    private void RemoveWaitRegistration(WaitRegistration registration)
    {
        _waitRegistrations.Remove(registration);

        registration.CancellationRegistration.Dispose();
    }

    private sealed class CancellationContext(EventObserver<T> observer, WaitRegistration registration)
    {
        public EventObserver<T> Observer { get; } = observer;
        public WaitRegistration Registration { get; } = registration;
    }

    private sealed class WaitRegistration
    {
        public WaitRegistration(Predicate<T>? predicate, int expectedCount, int matchedCount, TaskCompletionSource<bool> taskSource)
        {
            Predicate = predicate;
            ExpectedCount = expectedCount;
            MatchedCount = matchedCount;
            TaskSource = taskSource;
        }

        public Predicate<T>? Predicate { get; }
        public int ExpectedCount { get; }
        public int MatchedCount { get; set; }
        public TaskCompletionSource<bool> TaskSource { get; }
        public CancellationTokenRegistration CancellationRegistration { get; set; }
    }
}
