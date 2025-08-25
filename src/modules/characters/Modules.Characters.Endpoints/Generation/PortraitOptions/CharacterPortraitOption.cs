namespace Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

public record CharacterPortraitOption
{
    public required string Url { get; init; }
    public string? Description { get; init; }
}