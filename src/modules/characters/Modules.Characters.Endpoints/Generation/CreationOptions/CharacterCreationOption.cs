namespace Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;

public record CharacterCreationOption
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? ShortDescription { get; init; }
}