using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Elements.Labels.Update;

public sealed class UpdateElementLabelRequestValidator : Validator<UpdateElementLabelRequest>
{
    public UpdateElementLabelRequestValidator()
    {
        RuleFor(x => x.Labels)
            .NotNull()
            .WithMessage("Labels are required.");

        RuleForEach(x => x.Labels)
            .NotEmpty()
            .WithMessage("Label is required.")
            .MaximumLength(100)
            .WithMessage("Label must be 100 characters or less.");
    }
}
