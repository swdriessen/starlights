using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Proficiencies.Create;

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
    }
}
