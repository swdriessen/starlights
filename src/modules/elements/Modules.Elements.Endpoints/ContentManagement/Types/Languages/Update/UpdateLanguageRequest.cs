namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Languages.Update;

public sealed record UpdateLanguageRequest
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Kind { get; set; }
    public string? Origin { get; set; }
    public string Description { get; set; } = string.Empty;
}
