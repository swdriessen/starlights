using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Modules.Characters.Domain.Progression.Eventing;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Modules.Characters.Domain.SavingThrows.Eventing;
using Starlights.Modules.Characters.Domain.Skills.Eventing;
using Starlights.Platform.Eventing;

namespace Starlights.Integration.Tests.Core.Eventing;

internal sealed class IntegrationEventHandler :
    IDomainEventHandler<AbilityScoreCreatedEvent>,
    IDomainEventHandler<SkillCreatedEvent>,
    IDomainEventHandler<SavingThrowCreatedEvent>,
    IDomainEventHandler<CharacterClassCreatedEvent>,
    IDomainEventHandler<RegistrationSelectionRuleCreatedEvent>,
    IDomainEventHandler<RegistrationCreatedEvent>,
    IDomainEventHandler<CharacterLevelChangedEvent>
{
    private readonly IntegrationEventHandlerListener _listener;

    public IntegrationEventHandler(IntegrationEventHandlerListener listener)
    {
        _listener = listener;
    }

    public Task HandleAsync(AbilityScoreCreatedEvent domainEvent)
    {
        return _listener.AbilityScoreCreated.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(SkillCreatedEvent domainEvent)
    {
        return _listener.SkillCreated.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(SavingThrowCreatedEvent domainEvent)
    {
        return _listener.SavingThrowCreated.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(CharacterClassCreatedEvent domainEvent)
    {
        return _listener.CharacterClassCreated.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(RegistrationSelectionRuleCreatedEvent domainEvent)
    {
        return _listener.RegistrationSelectionRuleCreated.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(RegistrationCreatedEvent domainEvent)
    {
        return _listener.RegistrationCreated.Mock.Object.HandleAsync(domainEvent);
    }

    public Task HandleAsync(CharacterLevelChangedEvent domainEvent)
    {
        return _listener.CharacterLevelChanged.Mock.Object.HandleAsync(domainEvent);
    }
}
