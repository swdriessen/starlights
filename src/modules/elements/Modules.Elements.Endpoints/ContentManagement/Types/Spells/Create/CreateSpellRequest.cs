namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Spells.Create;

public record CreateSpellRequest
{
    public required string Name { get; set; }
    public int Level { get; set; }
    public required string MagicSchool { get; set; }
    public required string CastingTime { get; set; }
    public required string Range { get; set; }
    public required string Duration { get; set; }
    public bool IsConcentration { get; set; }
    public bool IsRitual { get; set; }
    public bool HasSomatic { get; set; }
    public bool HasVerbal { get; set; }
    public bool HasMaterial { get; set; }
    public string? MaterialComponent { get; set; }
    public string? Description { get; set; } = string.Empty;
}
