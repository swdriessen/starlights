using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures.Create;

public sealed class CreateClassFeatureRequestValidator : Validator<CreateClassFeatureRequest>
{
    public CreateClassFeatureRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.ParentClassId)
            .NotEmpty()
            .WithMessage("Parent class id is required.");

        RuleFor(x => x.ParentClassName)
            .NotEmpty()
            .WithMessage("Parent class name is required.");

        RuleFor(x => x.Level)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Level must be 0 or greater.");

        When(x => x.Description is not null, () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(10000)
                .WithMessage("Description is too long.");
        });
    }
}
