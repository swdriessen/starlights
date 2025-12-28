using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Create;

public sealed class CreateIncludeRuleRequestValidator : Validator<CreateIncludeRuleRequest>
{
    public CreateIncludeRuleRequestValidator()
    {
        RuleFor(x => x.ElementId).NotEmpty().WithMessage("ElementId is required.");
        RuleFor(x => x.IncludedElementId).NotEmpty().WithMessage("IncludedElementId is required.");
        RuleFor(x => x.LevelRequirement)
            .GreaterThanOrEqualTo(0)
            .WithMessage("LevelRequirement cannot be negative.");

        RuleFor(x => x.DisplayName)
            .MaximumLength(200)
            .WithMessage("DisplayName must be at most 200 characters.");

        RuleFor(x => x.RequirementsExpression)
            .MaximumLength(1000)
            .WithMessage("Requirements must be at most 1000 characters.");
    }
}
