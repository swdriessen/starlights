using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Elements.Labels.Delete;

public sealed class DeleteElementLabelRequestValidator : Validator<DeleteElementLabelRequest>
{
    public DeleteElementLabelRequestValidator()
    {
        RuleFor(x => x.Labels)
            .NotNull()
            .WithMessage("Labels are required.")
            .Must(x => x.Count > 0)
            .WithMessage("At least one label is required.");

        RuleForEach(x => x.Labels)
            .NotEmpty()
            .WithMessage("Label is required.")
            .MaximumLength(100)
            .WithMessage("Label must be 100 characters or less.");
    }
}
