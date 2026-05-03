using System.Diagnostics;
using Moq;
using Starlights.Platform.Eventing;

namespace Starlights.Integration.Eventing;

public class EventObserver<T> : IEventObserver where T : IDomainEvent
{
    private readonly CancellationToken _cancellationToken;
    private readonly object _gate = new();
    private readonly List<WaitRegistration> _waitRegistrations = [];

    public Mock<IDomainEventHandler<T>> Mock { get; } = new();
    public List<T> Events { get; } = [];

    public EventObserver(CancellationToken cancellationToken)
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
