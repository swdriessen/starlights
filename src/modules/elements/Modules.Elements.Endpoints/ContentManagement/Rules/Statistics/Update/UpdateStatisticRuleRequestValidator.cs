using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Update;

public sealed class UpdateStatisticRuleRequestValidator : Validator<UpdateStatisticRuleRequest>
{
    public UpdateStatisticRuleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Value)
            .NotEmpty();

        RuleFor(x => x.LevelRequirement)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Minimum)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Minimum.HasValue);

        RuleFor(x => x.Maximum)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Maximum.HasValue);

        RuleFor(x => x)
            .Must(x => !x.Minimum.HasValue || !x.Maximum.HasValue || x.Minimum.Value <= x.Maximum.Value)
            .WithMessage("Minimum must be less than or equal to Maximum.");
    }
}
