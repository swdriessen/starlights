namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Elements.Update;

public sealed record UpdateElementRequest
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }
}
