using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Feats.Update;

public sealed class UpdateFeatRequestValidator : Validator<UpdateFeatRequest>
{
    public UpdateFeatRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Category is required.");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.ShortDescription));
    }
}
