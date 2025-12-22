namespace Starlights.Modules.Elements.Endpoints.Content.FeatCategories.GetById;

public sealed record GetFeatCategoryByIdRequest
{
    public Guid Id { get; init; }
}
