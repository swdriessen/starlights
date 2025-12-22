using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.FeatCategories.Create;

public sealed class CreateFeatCategoryRequestValidator : Validator<CreateFeatCategoryRequest>
{
    public CreateFeatCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
    }
}
