namespace Starlights.Modules.Elements.Endpoints.Entities.Abilities.Create;

public record CreateAbilityRequest
{
    public string Name { get; init; } = string.Empty;
    public string Abbreviation { get; init; } = string.Empty;
}
