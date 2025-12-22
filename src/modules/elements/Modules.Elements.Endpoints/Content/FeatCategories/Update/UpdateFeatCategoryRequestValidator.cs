using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.FeatCategories.Update;

public sealed class UpdateFeatCategoryRequestValidator : Validator<UpdateFeatCategoryRequest>
{
    public UpdateFeatCategoryRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
    }
}
