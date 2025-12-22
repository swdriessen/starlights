namespace Starlights.Modules.Elements.Endpoints.Content.Languages.Create;

public record CreateLanguageRequest
{
    public required string Name { get; set; }
    public required string Kind { get; set; }
    public string? Origin { get; set; }
    public string? Description { get; set; } = string.Empty;
}
