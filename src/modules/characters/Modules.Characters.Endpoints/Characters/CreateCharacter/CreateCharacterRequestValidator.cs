using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Characters.Endpoints.Characters.CreateCharacter;

public sealed class CreateCharacterRequestValidator : Validator<CreateCharacterRequest>
{
    public CreateCharacterRequestValidator()
    {
        RuleFor(x => x.CharacterCreationOptionId)
            .NotEmpty().WithMessage("Character creation option is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Character name is required.")
            .MaximumLength(100).WithMessage("Character name cannot exceed 100 characters.");

        RuleFor(x => x.PortraitUrl)
            .MaximumLength(2048).WithMessage("Portrait URL cannot exceed 2048 characters.");
    }
}
