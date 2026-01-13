using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Reorder;

public sealed class ReorderIncludeRulesRequestValidator : Validator<ReorderIncludeRulesRequest>
{
    public ReorderIncludeRulesRequestValidator()
    {
        RuleFor(x => x.ElementId).NotEmpty().WithMessage("ElementId is required.");

        RuleFor(x => x.RuleIds)
            .NotNull()
            .WithMessage("RuleIds is required.");

        RuleFor(x => x.RuleIds)
            .Must(ids => ids.Count > 0)
            .WithMessage("At least one rule id must be provided.")
            .When(x => x.RuleIds is not null);

        RuleForEach(x => x.RuleIds)
            .NotEmpty()
            .WithMessage("RuleIds must not contain an empty id.");

        RuleFor(x => x.RuleIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("RuleIds must not contain duplicates.")
            .When(x => x.RuleIds is not null);
    }
}
