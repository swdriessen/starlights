namespace Starlights.Modules.Elements.Endpoints.Content.Spells.Update;

public sealed record UpdateSpellRequest
{
    public Guid Id { get; init; }
    public int Level { get; init; }
    public required string MagicSchool { get; init; }
    public required string CastingTime { get; init; }
    public required string Range { get; init; }
    public required string Duration { get; init; }
    public bool IsConcentration { get; init; }
    public bool IsRitual { get; init; }
    public bool HasSomatic { get; init; }
    public bool HasVerbal { get; init; }
    public bool HasMaterial { get; init; }
    public string? MaterialComponent { get; init; }
    public string? Description { get; init; }
}
