using System.Diagnostics;
using Starlights.Platform.Domain;

namespace Starlights.Modules.Characters.Domain;

/// <summary>
/// Represents a registration for an element in the system.
/// </summary>
[DebuggerDisplay("Id = {Id}, ElementName = {ElementName}, ElementId = {ElementId}")]
public sealed class Registration : AggregateRoot<RegistrationId>
{
    private Registration(CharacterId characterId, ElementId elementId, string elementName)
        : base(RegistrationId.New())
    {
        CharacterId = characterId;
        ElementId = elementId;
        ElementName = elementName;
    }

    /// <summary>
    /// Gets the ID of the character associated with this registration.
    /// </summary>
    public CharacterId CharacterId { get; }

    /// <summary>
    /// Gets the ID of the element associated with this registration.
    /// </summary>
    public ElementId ElementId { get; }

    /// <summary>
    /// Gets the name of the element associated with this registration. This is used for display purposes and may not be unique.
    /// </summary>
    public string ElementName { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="Registration"/> class with the specified character ID, element ID, and element name.
    /// </summary>
    public static Registration Create(CharacterId characterId, ElementId elementId, string elementName)
    {
        return new Registration(characterId, elementId, elementName);
    }
}
