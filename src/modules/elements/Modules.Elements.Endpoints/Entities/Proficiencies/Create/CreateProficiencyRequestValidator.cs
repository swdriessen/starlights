using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Entities.Proficiencies.Create;

public sealed class CreateProficiencyRequestValidator : Validator<CreateProficiencyRequest>
{
    public CreateProficiencyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.ProficiencyType)
            .NotEmpty()
            .WithMessage("Proficiency type is required.");

        RuleFor(x => x.Description)
            .MaximumLength(4000)
            .WithMessage("Description must be 4000 characters or less.");
    }
}
