namespace Starlights.Modules.Elements.Endpoints.Content.FeatCategories.Update;

public sealed record UpdateFeatCategoryRequest
{
    public required Guid Id { get; init; }

    public required string Name { get; set; }

    public string Description { get; init; } = string.Empty;
}
