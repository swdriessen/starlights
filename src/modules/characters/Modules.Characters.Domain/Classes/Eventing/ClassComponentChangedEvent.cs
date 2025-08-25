using Starlights.Modules.Characters.Domain.Characters.Eventing;

namespace Starlights.Modules.Characters.Domain.Classes.Eventing;

/// <summary>
/// Event raised when a character's class is changed.
/// </summary>
public sealed record ClassComponentChangedEvent : CharacterEventBase;