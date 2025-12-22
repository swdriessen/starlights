namespace Starlights.Modules.Elements.Endpoints.Content.FeatCategories.GetById;

public sealed record FeatCategoryDataModel
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public string Description { get; init; } = string.Empty;
}
