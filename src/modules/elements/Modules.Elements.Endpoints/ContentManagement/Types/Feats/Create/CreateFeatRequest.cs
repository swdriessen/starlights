namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Feats.Create;

public sealed record CreateFeatRequest
{
    public required string Name { get; set; }

    public required Guid CategoryId { get; set; }

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public bool IsRepeatable { get; set; }
    public string? Prerequisite { get; set; }
}
