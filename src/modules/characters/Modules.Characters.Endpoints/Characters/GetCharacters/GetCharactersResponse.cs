namespace Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;

sealed class GetCharactersResponse
{
    public List<CharacterDetailsDataModel> Characters { get; set; } = [];
}
