using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Proficiencies.Update;

public sealed class UpdateProficiencyRequestValidator : Validator<UpdateProficiencyRequest>
{
    public UpdateProficiencyRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

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
