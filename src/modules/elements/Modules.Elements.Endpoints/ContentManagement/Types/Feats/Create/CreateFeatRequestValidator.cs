using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Feats.Create;

public sealed class CreateFeatRequestValidator : Validator<CreateFeatRequest>
{
    public CreateFeatRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Category is required.");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.ShortDescription));
    }
}
