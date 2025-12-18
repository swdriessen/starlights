using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using Starlights.Integration.Core.Extensions;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters.Eventing;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Modules.Characters.Domain.Progression.Eventing;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Modules.Characters.Domain.SavingThrows.Eventing;
using Starlights.Modules.Characters.Domain.Skills.Eventing;
using Starlights.Platform.Eventing;

namespace Starlights.Integration.Core.Eventing;

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
    internal async Task EnsureObservation<T>(Predicate<T>? predicate = null, int count = 1) where T : IDomainEvent
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
    /// <remarks>Use this method to reset the invocation and event history for a particular domain event type,
    /// typically in test scenarios where event handling needs to be verified or reset between tests.</remarks>
    /// <typeparam name="T">The type of domain event whose invocations and events will be cleared. Must implement <see
    /// cref="IDomainEvent"/>.</typeparam>
    internal void ClearInvocations<T>() where T : IDomainEvent
    {
        var e = Event<T>();
        e.Mock.Invocations.Clear();
        e.Events.Clear();
    }

    /// <summary>
    /// Clears all recorded invocations for character-related events within the current context.
    /// </summary>
    /// <remarks>This method removes invocation records for multiple event types, including character
    /// creation, ability score creation, skill creation, saving throw creation, class changes, registration events, and
    /// level changes. Use this method to reset the invocation state before running new tests or processing new event
    /// sequences.</remarks>
    internal void ClearInvocations()
    {
        ClearInvocations<CharacterCreatedEvent>();
        ClearInvocations<AbilityScoreCreatedEvent>();
        ClearInvocations<SkillCreatedEvent>();
        ClearInvocations<SavingThrowCreatedEvent>();
        ClearInvocations<CharacterClassCreatedEvent>();
        ClearInvocations<CharacterClassRemovedEvent>();
        ClearInvocations<RegistrationSelectionRuleCreatedEvent>();
        ClearInvocations<RegistrationCreatedEvent>();
        ClearInvocations<CharacterLevelChangedEvent>();
        ClearInvocations<RegistrationProcessedEvent>();
    }
}

public class EventObserverT<T> where T : IDomainEvent
{
    private readonly CancellationToken _cancellationToken;

    public Mock<IDomainEventHandler<T>> Mock { get; } = new();
    public List<T> Events { get; } = [];

    public EventObserverT(CancellationToken cancellationToken)
    {
        Mock.Setup(m => m.HandleAsync(It.IsAny<T>()))
            .Callback<T>(Events.Add)
            .Returns(Task.CompletedTask);

        _cancellationToken = cancellationToken;
    }

    public Task HandleAsync(T domainEvent)
    {
        return Mock.Object.HandleAsync(domainEvent);
    }

    public Task WaitForEvent(Predicate<T>? predicate = null, int count = 1)
    {
        Trace.WriteLine($"[TRC] waiting for event {typeof(T).Name} .... current invocations: {Mock.Invocations.Count}");
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        var callCount = 0;

        Mock.Setup(m => m.HandleAsync(It.IsAny<T>()))
            .Callback<T>(evt =>
            {
                Events.Add(evt);
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

        _cancellationToken.Register(() =>
        {
            tcs.TrySetCanceled(_cancellationToken);
            Trace.WriteLine($"[TRC] waiting for event {typeof(T).Name} was cancelled due to test timeout");
        });

        return tcs.Task;
    }
}
