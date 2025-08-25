namespace Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;

public record GetCharacterCreationOptionsResponse
{
    public List<CharacterCreationOption> Options { get; init; } = [];
}
