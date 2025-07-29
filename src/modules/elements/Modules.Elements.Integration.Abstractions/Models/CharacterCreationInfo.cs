namespace Starlights.Modules.Elements.Integration.Abstractions.Models;

/// <summary>
/// The DTO model for character creation information.
/// </summary>
public record CharacterCreationInfo
{
    public CharacterCreationInfo(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public string? ShortDescription { get; init; }
}
