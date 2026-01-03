namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Spells;

/// <summary>
/// The DTO for a spell.
/// </summary>
public sealed record SpellDataModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int Level { get; init; }
    public required string MagicSchool { get; init; }
    public required string CastingTime { get; init; }
    public required string Range { get; init; }
    public required string Duration { get; init; }
    public required bool IsConcentration { get; init; }
    public required bool IsRitual { get; init; }
    public required bool HasSomatic { get; init; }
    public required bool HasVerbal { get; init; }
    public required bool HasMaterial { get; init; }
    public string? MaterialComponent { get; init; }
    public required string Description { get; init; }
}
