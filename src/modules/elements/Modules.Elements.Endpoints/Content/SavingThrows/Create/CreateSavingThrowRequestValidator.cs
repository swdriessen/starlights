using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.SavingThrows.Create;

public sealed class CreateSavingThrowRequestValidator : Validator<CreateSavingThrowRequest>
{
    public CreateSavingThrowRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.AbilityId)
            .NotEmpty().WithMessage("AbilityId is required.");
    }
}
