using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Integration.Abstractions.Models;

namespace Starlights.Modules.Elements.Extensions;

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
}