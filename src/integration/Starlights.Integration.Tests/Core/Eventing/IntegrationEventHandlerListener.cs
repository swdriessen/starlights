using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.SavingThrows.Eventing;
using Starlights.Modules.Characters.Domain.Skills.Eventing;

namespace Starlights.Integration.Tests.Core.Eventing;

public sealed class IntegrationEventHandlerListener
{
    public IntegrationEventHandlerListener<AbilityScoreCreatedEvent> AbilityScoreCreated { get; } = new();
    public IntegrationEventHandlerListener<SkillCreatedEvent> SkillCreated { get; } = new();
    public IntegrationEventHandlerListener<SavingThrowCreatedEvent> SavingThrowCreated { get; } = new();
}
