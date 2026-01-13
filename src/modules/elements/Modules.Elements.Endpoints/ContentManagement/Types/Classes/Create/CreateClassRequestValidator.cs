using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes.Create;

public class CreateClassRequestValidator : Validator<CreateClassRequest>
{
    public CreateClassRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.HitPointDieSize)
            .GreaterThan(0)
            .WithMessage("Hit point die size must be greater than 0.");

        RuleFor(x => x.HitPointDieAmount)
            .GreaterThan(0)
            .WithMessage("Hit point die amount must be greater than 0.");
    }
}
