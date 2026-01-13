using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.AbilityScores.Delete;

public sealed class DeleteAbilityScoreRequestValidator : Validator<DeleteAbilityScoreRequest>
{
    public DeleteAbilityScoreRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
