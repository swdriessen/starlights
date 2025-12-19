namespace Starlights.Integration.Drivers.Elements;

internal sealed record CreateSpellProperties
{
    public required string Name { get; set; }
    public required int Level { get; set; }
    public required string School { get; set; }
    public required string Time { get; set; }
    public required string Range { get; set; }
    public required string Duration { get; set; }
    public bool IsConcentration { get; set; }
    public bool IsRitual { get; set; }
    public bool HasSomatic { get; set; }
    public bool HasVerbal { get; set; }
    public bool HasMaterial { get; set; }
    public string? MaterialComponent { get; set; }
    public string Description { get; set; } = string.Empty;
}