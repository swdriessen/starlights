namespace Starlights.Modules.Elements.Integration.Models;

/// <summary>
/// The DTO model for character creation information.
/// </summary>
public record CharacterCreationDataModel
{
    public CharacterCreationDataModel(Guid elementId, string name, string type)
    {
        ElementId = elementId;
        Name = name;
        Type = type;
    }

    public Guid ElementId { get; init; }
    public string Name { get; init; }
    public string Type { get; init; }
    public string? ShortDescription { get; init; }
}
