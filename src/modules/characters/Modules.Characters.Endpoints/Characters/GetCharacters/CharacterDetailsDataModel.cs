namespace Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;

/// <summary>
/// The DTO model for a simple character details model used in lists.
/// </summary>
public record CharacterDetailsDataModel
{
    public required Guid CharacterId { get; set; }
    public required string Name { get; set; }
    public string? PortraitUrl { get; set; }
    public int? Level { get; set; }
    public string? Build { get; set; }
}
