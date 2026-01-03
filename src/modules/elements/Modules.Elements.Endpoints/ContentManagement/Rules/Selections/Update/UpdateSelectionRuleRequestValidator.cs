using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.Update;

public sealed class UpdateSelectionRuleRequestValidator : Validator<UpdateSelectionRuleRequest>
{
    public UpdateSelectionRuleRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .WithMessage("display name is required");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("type is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("quantity must be at least 1");

        RuleFor(x => x.LevelRequirement)
            .GreaterThanOrEqualTo(0)
            .WithMessage("level requirement cannot be negative");
    }
}
