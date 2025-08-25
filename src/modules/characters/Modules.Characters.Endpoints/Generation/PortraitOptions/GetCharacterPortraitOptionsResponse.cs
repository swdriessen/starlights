namespace Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

public record GetCharacterPortraitOptionsResponse
{
    public List<CharacterPortraitOption> Portraits { get; init; } = [];
}
