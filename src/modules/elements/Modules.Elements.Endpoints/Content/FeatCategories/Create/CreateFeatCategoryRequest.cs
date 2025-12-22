namespace Starlights.Modules.Elements.Endpoints.Content.FeatCategories.Create;

public sealed record CreateFeatCategoryRequest
{
    public required string Name { get; set; }

    public string? Description { get; set; }
}
