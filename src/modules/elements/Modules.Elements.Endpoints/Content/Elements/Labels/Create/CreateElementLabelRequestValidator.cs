using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Elements.Labels.Create;

public sealed class CreateElementLabelRequestValidator : Validator<CreateElementLabelRequest>
{
    public CreateElementLabelRequestValidator()
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
