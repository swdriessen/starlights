using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Attributes.Spells.GetById;

public sealed class GetSpellByIdRequestValidator : Validator<GetSpellByIdRequest>
{
    public GetSpellByIdRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}
