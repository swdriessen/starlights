using Starlights.Modules.Characters.Domain.Registrations.Eventing;

namespace Starlights.Modules.Characters.Domain.Abilities.Eventing;

/// <summary>
/// Represents an event that is raised when an ability element registration is completed.
/// </summary>
public record AbilityElementRegistrationCreatedEvent : RegistrationCreatedEvent;

/// <summary>
/// Represents an event that is raised when a skill element registration is completed.
/// </summary>
public record SkillElementRegistrationCreatedEvent : RegistrationCreatedEvent;