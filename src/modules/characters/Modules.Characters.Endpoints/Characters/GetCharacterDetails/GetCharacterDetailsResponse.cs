using Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;

namespace Starlights.Modules.Characters.Endpoints.Characters.GetCharacterDetails;

public sealed class GetCharacterDetailsResponse
{
    public required CharacterDetailsDataModel Character { get; init; }
}
