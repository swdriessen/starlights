namespace Starlights.Modules.Elements.Endpoints.Content.Elements.Update;

public sealed record UpdateElementRequest
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }
}
