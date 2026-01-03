using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Reorder;

public sealed class ReorderElementRulesRequestValidator : Validator<ReorderElementRulesRequest>
{
    public ReorderElementRulesRequestValidator()
    {
        RuleFor(x => x.RuleIds)
            .NotNull().WithMessage("rule ids are required")
            .NotEmpty().WithMessage("rule ids must not be empty")
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("rule ids must be unique");
    }
}
