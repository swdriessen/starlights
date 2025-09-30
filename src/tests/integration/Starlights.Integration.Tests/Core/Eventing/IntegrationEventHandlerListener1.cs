using Moq;
using Starlights.Platform.Eventing;

namespace Starlights.Integration.Tests.Core.Eventing;

public class IntegrationEventHandlerListener<T> where T : IDomainEvent
{
    public Mock<IDomainEventHandler<T>> Mock { get; } = new();
    public List<T> Events { get; } = [];

    public IntegrationEventHandlerListener()
    {
        Mock.Setup(m => m.HandleAsync(It.IsAny<T>()))
            .Callback<T>(Events.Add)
            .Returns(Task.CompletedTask);
    }

    public Task HandleAsync(T domainEvent) => Mock.Object.HandleAsync(domainEvent);

    public Task WaitForEvent(Predicate<T>? predicate = null, int count = 1, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        int callCount = 0;

        Mock.Setup(m => m.HandleAsync(It.IsAny<T>()))
            .Callback<T>(evt =>
            {
                if (predicate == null || predicate(evt))
                {
                    if (Interlocked.Increment(ref callCount) >= count)
                    {
                        tcs.TrySetResult(true);
                    }
                }
            })
            .Returns(Task.CompletedTask);

        // check if the task condition is already completed before the wait
        if (Mock.Invocations.Count > 0)
        {
            // check if any of the invocations match the predicate
            if (Mock.Invocations.Any(inv => inv.Arguments[0] is T evt && (predicate == null || predicate(evt))))
            {
                if (Interlocked.Increment(ref callCount) >= count)
                {
                    tcs.TrySetResult(true);
                }
            }
        }

        cancellationToken.Register(() => tcs.TrySetCanceled());

        return tcs.Task;
    }
}
