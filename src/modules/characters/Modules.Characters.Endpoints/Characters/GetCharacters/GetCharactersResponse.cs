namespace Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;

/// <summary>A bounded page of the owner's saved Characters.</summary>
public sealed record GetCharactersResponse
{
    /// <summary>Gets saved Character summaries.</summary>
    public List<CharacterDetailsDataModel> Characters { get; init; } = [];
    /// <summary>Gets the next page offset, or null at the end of the collection.</summary>
    public int? NextOffset { get; init; }
}
