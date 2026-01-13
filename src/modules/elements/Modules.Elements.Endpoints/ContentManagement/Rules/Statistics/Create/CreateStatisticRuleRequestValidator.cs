using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Create;

public sealed class CreateStatisticRuleRequestValidator : Validator<CreateStatisticRuleRequest>
{
    public CreateStatisticRuleRequestValidator()
    {
        RuleFor(x => x.ElementId)
            .NotEmpty();

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
    }
}
