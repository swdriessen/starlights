namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.Helpers;

internal record CreateSpellTableRow
{
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public string MagicSchool { get; set; } = string.Empty;
    public string CastingTime { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public bool Concentration { get; set; }
    public bool Ritual { get; set; }
    public bool Somatic { get; set; }
    public bool Verbal { get; set; }
    public bool Material { get; set; }
    public string? MaterialComponents { get; set; }
    public string Description { get; set; } = string.Empty;
}
