namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Feats;

/// <summary>
/// The DTO representing a feat.
/// </summary>
public sealed record FeatDataModel
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required Guid CategoryId { get; init; }

    public required string Category { get; init; }

    public string? ShortDescription { get; init; }

    public required string Description { get; init; } = string.Empty;

    public string? Prerequisites { get; init; }

    public bool IsRepeatable { get; init; }
}
