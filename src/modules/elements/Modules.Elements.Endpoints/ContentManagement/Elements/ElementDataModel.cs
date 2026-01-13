namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Elements;

/// <summary>
/// The generic DTO representing an element.
/// </summary>
public sealed record ElementDataModel
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Type { get; init; }

    public required string Description { get; init; } = string.Empty;
}
