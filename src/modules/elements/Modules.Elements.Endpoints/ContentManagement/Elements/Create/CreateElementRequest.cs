namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Elements.Create;

public record CreateElementRequest
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public string? Description { get; set; }
}
