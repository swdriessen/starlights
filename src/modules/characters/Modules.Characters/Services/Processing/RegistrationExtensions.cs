using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Modules.Characters.Services.Processing;

public static class RegistrationExtensions
{
    public static void IncludeSpecificEvents(this Registration registration)
    {
        if (registration.AssociatedElementType == "Ability")
        {
            //registration.WithAdditionalEvent(new AbilityElementRegistrationCreatedEvent
            //{
            //    CharacterId = registration.CharacterId,
            //    RegistrationId = registration.Id,
            //    AssociatedElementName = registration.AssociatedElementName,
            //    AssociatedElementType = registration.AssociatedElementType
            //});
        }
        else if (registration.AssociatedElementType == "Skill")
        {
            registration.WithAdditionalEvent(new SkillElementRegistrationCreatedEvent
            {
                CharacterId = registration.CharacterId,
                RegistrationId = registration.Id,
                AssociatedElementName = registration.AssociatedElementName,
                AssociatedElementType = registration.AssociatedElementType
            });
        }
    }
}