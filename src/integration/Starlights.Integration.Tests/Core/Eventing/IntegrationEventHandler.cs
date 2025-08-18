using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Skills.Eventing;
using Starlights.Platform.Eventing;

namespace Starlights.Integration.Tests.Core.Eventing;

internal sealed class IntegrationEventHandler :
    IDomainEventHandler<AbilityScoreCreatedEvent>,
    IDomainEventHandler<SkillCreatedEvent>
{
    private readonly IntegrationEventHandlerListener _listener;

    public IntegrationEventHandler(IntegrationEventHandlerListener listener)
    {
        _listener = listener;
    }

    public Task HandleAsync(AbilityScoreCreatedEvent domainEvent) => _listener.AbilityScoreCreated.Mock.Object.HandleAsync(domainEvent);
    public Task HandleAsync(SkillCreatedEvent domainEvent) => _listener.SkillCreated.Mock.Object.HandleAsync(domainEvent);
}
