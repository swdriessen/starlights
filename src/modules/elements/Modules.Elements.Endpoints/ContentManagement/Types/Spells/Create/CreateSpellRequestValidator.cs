using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Spells.Create;

public class CreateSpellRequestValidator : Validator<CreateSpellRequest>
{
    public CreateSpellRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.MagicSchool).NotEmpty().WithMessage("Magic school is required.");
        RuleFor(x => x.CastingTime).NotEmpty().WithMessage("Casting time is required.");
        RuleFor(x => x.Range).NotEmpty().WithMessage("Range is required.");
        RuleFor(x => x.Duration).NotEmpty().WithMessage("Duration is required.");
    }
}
