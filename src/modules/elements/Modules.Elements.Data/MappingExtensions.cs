using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Integration.Abstractions.Models;

namespace Starlights.Modules.Elements.Data;

public static class ElementMappingExtensions
{
    public static CharacterCreationInfo AsCharacterCreationInfo(this Element element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));

        var description = element.Components
            .OfType<ShortDescriptionComponent>()
            .SingleOrDefault();

        return new CharacterCreationInfo(element.Id, element.Name)
        {
            ShortDescription = description?.Content
        };
    }

    public static AbilityInfo AsAbilityInfo(this Element element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));

        var abbreviationComponent = element.GetComponent<AbbreviationComponent>();

        return new AbilityInfo(element.Id, element.Name, abbreviationComponent.Abbreviation);
    }

    public static ElementInfo AsElementInfo(this Element element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));

        return new ElementInfo(element.Name, element.Type, "Internal", element.Id);
    }
}