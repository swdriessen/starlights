using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.FeatCategories.GetById;

public sealed class GetFeatCategoryByIdRequestValidator : Validator<GetFeatCategoryByIdRequest>
{
    public GetFeatCategoryByIdRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}
