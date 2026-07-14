using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters.Eventing;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Modules.Characters.Domain.Progression.Eventing;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Modules.Characters.Domain.SavingThrows.Eventing;
using Starlights.Modules.Characters.Domain.Skills.Eventing;
using Starlights.Modules.Elements.Domain.Eventing;
using Starlights.Platform.Eventing;

namespace Starlights.Integration.Eventing;

internal sealed class IntegrationEventHandler :
    IDomainEventHandler<CharacterCreatedEvent>,
    IDomainEventHandler<AbilityScoreCreatedEvent>,
    IDomainEventHandler<SkillCreatedEvent>,
    IDomainEventHandler<SavingThrowCreatedEvent>,
    IDomainEventHandler<CharacterClassCreatedEvent>,
    IDomainEventHandler<RegistrationSelectionRuleCreatedEvent>,
    IDomainEventHandler<RegistrationStatisticRuleCreatedEvent>,
    IDomainEventHandler<RegistrationCreatedEvent>,
    IDomainEventHandler<CharacterLevelChangedEvent>,
    IDomainEventHandler<CharacterClassRemovedEvent>,
    IDomainEventHandler<RegistrationProcessedEvent>,
    IDomainEventHandler<ElementCreatedEvent>
{
    private readonly EventObserverCollection _observers;

    public IntegrationEventHandler(EventObserverCollection observers)
    {
        _observers = observers;
    }

    public Task HandleAsync(CharacterCreatedEvent domainEvent)
    {
        return _observers.HandleAsync(domainEvent);
    }

    public Task HandleAsync(AbilityScoreCreatedEvent domainEvent)
    {
        return _observers.HandleAsync(domainEvent);
    }

    public Task HandleAsync(SkillCreatedEvent domainEvent)
    {
        return _observers.HandleAsync(domainEvent);
    }

    public Task HandleAsync(SavingThrowCreatedEvent domainEvent)
    {
        return _observers.HandleAsync(domainEvent);
    }

    public Task HandleAsync(CharacterClassCreatedEvent domainEvent)
    {
        return _observers.HandleAsync(domainEvent);
    }

    public Task HandleAsync(CharacterClassRemovedEvent domainEvent)
    {
        return _observers.HandleAsync(domainEvent);
    }

    public Task HandleAsync(RegistrationSelectionRuleCreatedEvent domainEvent)
    {
        return _observers.HandleAsync(domainEvent);
    }

    public Task HandleAsync(RegistrationStatisticRuleCreatedEvent domainEvent)
    {
        return _observers.HandleAsync(domainEvent);
    }

    public Task HandleAsync(RegistrationCreatedEvent domainEvent)
    {
        return _observers.HandleAsync(domainEvent);
    }

    public Task HandleAsync(CharacterLevelChangedEvent domainEvent)
    {
        return _observers.HandleAsync(domainEvent);
    }

    public Task HandleAsync(RegistrationProcessedEvent domainEvent)
    {
        return _observers.HandleAsync(domainEvent);
    }

    public Task HandleAsync(ElementCreatedEvent domainEvent)
    {
        return _observers.HandleAsync(domainEvent);
    }
}
