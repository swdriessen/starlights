using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses.Create;

public sealed class CreateSubClassRequestValidator : Validator<CreateSubClassRequest>
{
    public CreateSubClassRequestValidator()
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

        When(x => x.Description is not null, () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(10000)
                .WithMessage("Description is too long.");
        });
    }
}
