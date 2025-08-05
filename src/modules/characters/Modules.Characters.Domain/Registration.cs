using Starlights.Platform.Domain;

namespace Starlights.Modules.Characters.Domain;

public sealed class Registration : AggregateRoot<RegistrationId>
{
    private Registration(CharacterId characterId, ElementId elementId, string elementName)
        : base(RegistrationId.New())
    {
        CharacterId = characterId;
        ElementId = elementId;
        ElementName = elementName;
    }

    public CharacterId CharacterId { get; }
    public ElementId ElementId { get; }
    public string ElementName { get; }

    public static Registration Create(CharacterId characterId, ElementId elementId, string elementName)
    {
        return new Registration(characterId, elementId, elementName);
    }
}
