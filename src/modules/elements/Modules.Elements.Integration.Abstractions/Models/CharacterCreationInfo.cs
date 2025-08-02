namespace Starlights.Modules.Elements.Integration.Models;

/// <summary>
/// The DTO model for character creation information.
/// </summary>
public record CharacterCreationInfo
{
    public CharacterCreationInfo(Guid elementId, string name)
    {
        ElementId = elementId;
        Name = name;
    }

    public Guid ElementId { get; init; }
    public string Name { get; init; }
    public string? ShortDescription { get; init; }
}
