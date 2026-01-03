using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Delete;

public sealed class DeleteElementRulesRequestValidator : Validator<DeleteElementRulesRequest>
{
    public DeleteElementRulesRequestValidator()
    {
        RuleFor(x => x.ElementId)
            .NotEmpty().WithMessage("ElementId is required.");

        RuleFor(x => x.RuleIds)
            .NotNull().WithMessage("RuleIds is required.")
            .Must(x => x is { Count: > 0 }).WithMessage("At least one rule id is required.");

        RuleForEach(x => x.RuleIds)
            .NotEmpty().WithMessage("RuleIds cannot contain empty values.");
    }
}
