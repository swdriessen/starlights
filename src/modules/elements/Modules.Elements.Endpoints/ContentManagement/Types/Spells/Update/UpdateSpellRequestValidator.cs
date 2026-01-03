using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Spells.Update;

public sealed class UpdateSpellRequestValidator : Validator<UpdateSpellRequest>
{
    public UpdateSpellRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Level).GreaterThanOrEqualTo(0).WithMessage("Level cannot be negative.");

        RuleFor(x => x.MagicSchool).NotEmpty().WithMessage("Magic school is required.");
        RuleFor(x => x.CastingTime).NotEmpty().WithMessage("Casting time is required.");
        RuleFor(x => x.Range).NotEmpty().WithMessage("Range is required.");
        RuleFor(x => x.Duration).NotEmpty().WithMessage("Duration is required.");

        When(x => x.HasMaterial, () =>
        {
            RuleFor(x => x.MaterialComponent)
                .NotEmpty()
                .WithMessage("Material component is required when HasMaterial is true.");
        });
    }
}
