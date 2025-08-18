using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Skills;

namespace Starlights.Integration.Tests.Core.Eventing;

public sealed class IntegrationEventHandlerListener
{
    public IntegrationEventHandlerListener<AbilityScoreCreatedEvent> AbilityScoreCreated { get; } = new();
    public IntegrationEventHandlerListener<SkillCreatedEvent> SkillCreated { get; } = new();
}
