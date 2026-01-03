namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Feats.Update;

public sealed record UpdateFeatRequest
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required Guid CategoryId { get; set; }

    public string? Prerequisite { get; set; } = string.Empty;

    public bool IsRepeatable { get; set; }

    public string? ShortDescription { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}