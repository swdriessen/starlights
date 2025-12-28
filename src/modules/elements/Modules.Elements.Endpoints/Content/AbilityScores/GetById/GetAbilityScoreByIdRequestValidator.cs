using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.AbilityScores.GetById;

public sealed class GetAbilityScoreByIdRequestValidator : Validator<GetAbilityScoreByIdRequest>
{
    public GetAbilityScoreByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
