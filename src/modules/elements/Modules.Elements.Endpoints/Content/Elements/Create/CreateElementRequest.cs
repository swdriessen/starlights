namespace Starlights.Modules.Elements.Endpoints.Content.Elements.Create;

public record CreateElementRequest
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public string? Description { get; set; }
}
