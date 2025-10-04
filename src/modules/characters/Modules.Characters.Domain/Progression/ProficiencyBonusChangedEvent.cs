using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Progression;

public record ProficiencyBonusChangedEvent(int NewBonus) : CharacterEventBase;