using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using Starlights.Integration.Extensions;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters.Eventing;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Modules.Characters.Domain.Progression.Eventing;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Modules.Characters.Domain.SavingThrows.Eventing;
using Starlights.Modules.Characters.Domain.Skills.Eventing;
using Starlights.Platform.Eventing;

namespace Starlights.Integration.Eventing;

public sealed class EventObserverCollection
{
    private readonly ILogger<EventObserverCollection> _logger;
    private readonly IIntegrationHost _integration;
    private readonly CancellationToken _cancellationToken;

    public EventObserverCollection(ILogger<EventObserverCollection> logger, IIntegrationHost integration)
    {
        _logger = logger;
        _integration = integration;
        _cancellationToken = _integration.CancellationToken;

        CharacterCreated = new(_cancellationToken);
        AbilityScoreCreated = new(_cancellationToken);
        SkillCreated = new(_cancellationToken);
        SavingThrowCreated = new(_cancellationToken);
        CharacterClassCreated = new(_cancellationToken);
        CharacterClassRemoved = new(_cancellationToken);
        RegistrationSelectionRuleCreated = new(_cancellationToken);
        RegistrationStatisticRuleCreated = new(_cancellationToken);
        RegistrationCreated = new(_cancellationToken);
        CharacterLevelChanged = new(_cancellationToken);
        RegistrationProcessed = new(_cancellationToken);
    }


    public EventObserverT<CharacterCreatedEvent> CharacterCreated { get; }
    public EventObserverT<AbilityScoreCreatedEvent> AbilityScoreCreated { get; }
    public EventObserverT<SkillCreatedEvent> SkillCreated { get; }
    public EventObserverT<SavingThrowCreatedEvent> SavingThrowCreated { get; }
    public EventObserverT<CharacterClassCreatedEvent> CharacterClassCreated { get; }
    public EventObserverT<CharacterClassRemovedEvent> CharacterClassRemoved { get; }
    public EventObserverT<RegistrationSelectionRuleCreatedEvent> RegistrationSelectionRuleCreated { get; }
    public EventObserverT<RegistrationStatisticRuleCreatedEvent> RegistrationStatisticRuleCreated { get; }
    public EventObserverT<RegistrationCreatedEvent> RegistrationCreated { get; }
    public EventObserverT<CharacterLevelChangedEvent> CharacterLevelChanged { get; }
    public EventObserverT<RegistrationProcessedEvent> RegistrationProcessed { get; }

    private EventObserverT<T> Event<T>() where T : IDomainEvent
    {
        return typeof(T) switch
        {
            _ when typeof(T) == typeof(CharacterCreatedEvent) => (EventObserverT<T>)(object)CharacterCreated,
            _ when typeof(T) == typeof(AbilityScoreCreatedEvent) => (EventObserverT<T>)(object)AbilityScoreCreated,
            _ when typeof(T) == typeof(SkillCreatedEvent) => (EventObserverT<T>)(object)SkillCreated,
            _ when typeof(T) == typeof(SavingThrowCreatedEvent) => (EventObserverT<T>)(object)SavingThrowCreated,
            _ when typeof(T) == typeof(CharacterClassCreatedEvent) => (EventObserverT<T>)(object)CharacterClassCreated,
            _ when typeof(T) == typeof(CharacterClassRemovedEvent) => (EventObserverT<T>)(object)CharacterClassRemoved,
            _ when typeof(T) == typeof(RegistrationSelectionRuleCreatedEvent) => (EventObserverT<T>)(object)RegistrationSelectionRuleCreated,
            _ when typeof(T) == typeof(RegistrationStatisticRuleCreatedEvent) => (EventObserverT<T>)(object)RegistrationStatisticRuleCreated,
            _ when typeof(T) == typeof(RegistrationCreatedEvent) => (EventObserverT<T>)(object)RegistrationCreated,
            _ when typeof(T) == typeof(CharacterLevelChangedEvent) => (EventObserverT<T>)(object)CharacterLevelChanged,
            _ when typeof(T) == typeof(RegistrationProcessedEvent) => (EventObserverT<T>)(object)RegistrationProcessed,
            _ => throw new NotSupportedException($"No event listener registered for event type {typeof(T).FullName}.")
        };
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
        var e = Event<T>();
        e.Mock.Invocations.Clear();
        e.Events.Clear();
    }

    /// <summary>
    /// Clears all recorded invocations for character-related events within the current context.
    /// </summary>
    public void ClearInvocations()
    {
        ClearInvocations<CharacterCreatedEvent>();
        ClearInvocations<AbilityScoreCreatedEvent>();
        ClearInvocations<SkillCreatedEvent>();
        ClearInvocations<SavingThrowCreatedEvent>();
        ClearInvocations<CharacterClassCreatedEvent>();
        ClearInvocations<CharacterClassRemovedEvent>();
        ClearInvocations<RegistrationSelectionRuleCreatedEvent>();
        ClearInvocations<RegistrationStatisticRuleCreatedEvent>();
        ClearInvocations<RegistrationCreatedEvent>();
        ClearInvocations<CharacterLevelChangedEvent>();
        ClearInvocations<RegistrationProcessedEvent>();
    }
}

public class EventObserverT<T> where T : IDomainEvent
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
