namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.FeatCategories.GetById;

public sealed record GetFeatCategoryByIdRequest
{
    public Guid Id { get; init; }
}
