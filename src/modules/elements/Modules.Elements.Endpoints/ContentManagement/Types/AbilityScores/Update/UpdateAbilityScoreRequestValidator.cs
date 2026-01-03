using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.Update;

public sealed class UpdateAbilityScoreRequestValidator : Validator<UpdateAbilityScoreRequest>
{
    public UpdateAbilityScoreRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

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
