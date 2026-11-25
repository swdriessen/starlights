using Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;

namespace Starlights.Modules.Characters.Endpoints.Characters.GetCharacterDetails;

/// <summary>An owner-authorized saved Character overview.</summary>
public sealed record GetCharacterDetailsResponse
{
    public required CharacterDetailsDataModel Character { get; init; }
}
