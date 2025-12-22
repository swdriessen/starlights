namespace Starlights.Integration.Acceptance.Tests.StepDefinitions.Helpers;

internal record UpdateSpellTableRow : IMarkdownDescriptionTableRow
{
    public string? Name { get; set; }
    public int? Level { get; set; }
    public string? MagicSchool { get; set; }
    public string? CastingTime { get; set; }
    public string? Range { get; set; }
    public string? Duration { get; set; }
    public bool? Concentration { get; set; }
    public bool? Ritual { get; set; }
    public bool? Somatic { get; set; }
    public bool? Verbal { get; set; }
    public bool? Material { get; set; }
    public string? MaterialComponents { get; set; }
    public string? Description { get; set; }
}