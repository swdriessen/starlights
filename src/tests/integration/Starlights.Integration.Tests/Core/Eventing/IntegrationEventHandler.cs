using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters.Eventing;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Modules.Characters.Domain.Progression.Eventing;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Modules.Characters.Domain.SavingThrows.Eventing;
using Starlights.Modules.Characters.Domain.Skills.Eventing;
using Starlights.Platform.Eventing;

namespace Starlights.Integration.Core.Eventing;

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
    IDomainEventHandler<RegistrationProcessedEvent>
{
    private readonly EventObserverCollection _observers;

    public IntegrationEventHandler(EventObserverCollection observers)
    {
        _observers = observers;
    }

    public Task HandleAsync(CharacterCreatedEvent domainEvent)
    {
        return _observers.CharacterCreated.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(AbilityScoreCreatedEvent domainEvent)
    {
        return _observers.AbilityScoreCreated.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(SkillCreatedEvent domainEvent)
    {
        return _observers.SkillCreated.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(SavingThrowCreatedEvent domainEvent)
    {
        return _observers.SavingThrowCreated.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(CharacterClassCreatedEvent domainEvent)
    {
        return _observers.CharacterClassCreated.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(CharacterClassRemovedEvent domainEvent)
    {
        return _observers.CharacterClassRemoved.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(RegistrationSelectionRuleCreatedEvent domainEvent)
    {
        return _observers.RegistrationSelectionRuleCreated.Mock.Object.HandleAsync(domainEvent);
    }
    public Task HandleAsync(RegistrationStatisticRuleCreatedEvent domainEvent)
    {
        return _observers.RegistrationStatisticRuleCreated.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(RegistrationCreatedEvent domainEvent)
    {
        return _observers.RegistrationCreated.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(CharacterLevelChangedEvent domainEvent)
    {
        // TODO: try make this work with the generic method
        //return _listener.Event<CharacterLevelChangedEvent>().Mock.Object.HandleAsync(domainEvent);

        return _observers.CharacterLevelChanged.Mock.Object.HandleAsync(domainEvent);
    }
    public Task HandleAsync(RegistrationProcessedEvent domainEvent)
    {
        return _observers.RegistrationProcessed.Mock.Object.HandleAsync(domainEvent);
    }
}
