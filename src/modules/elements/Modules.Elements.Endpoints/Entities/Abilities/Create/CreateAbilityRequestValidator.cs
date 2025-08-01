using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Entities.Abilities.Create;

public class CreateAbilityRequestValidator : Validator<CreateAbilityRequest>
{
    public CreateAbilityRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        RuleFor(x => x.Abbreviation)
            .NotEmpty().WithMessage("Abbreviation is required.")
            .MaximumLength(10).WithMessage("Abbreviation must not exceed 10 characters.");
    }
}
