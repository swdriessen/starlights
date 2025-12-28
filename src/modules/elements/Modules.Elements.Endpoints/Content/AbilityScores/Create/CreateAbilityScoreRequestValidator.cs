using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.AbilityScores.Create;

public sealed class CreateAbilityScoreRequestValidator : Validator<CreateAbilityScoreRequest>
{
    public CreateAbilityScoreRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must be at most 200 characters.");

        RuleFor(x => x.Abbreviation)
            .NotEmpty().WithMessage("Abbreviation is required.")
            .MaximumLength(10).WithMessage("Abbreviation must be at most 10 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must be at most 2000 characters.");
    }
}
