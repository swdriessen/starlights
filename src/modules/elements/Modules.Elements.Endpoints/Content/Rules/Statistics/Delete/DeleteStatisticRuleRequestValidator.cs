using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Delete;

public sealed class DeleteStatisticRuleRequestValidator : Validator<DeleteStatisticRuleRequest>
{
    public DeleteStatisticRuleRequestValidator()
    {
        RuleFor(x => x.ElementId)
            .NotEmpty().WithMessage("ElementId is required.");

        RuleFor(x => x.RuleId)
            .NotEmpty().WithMessage("RuleId is required.");
    }
}
