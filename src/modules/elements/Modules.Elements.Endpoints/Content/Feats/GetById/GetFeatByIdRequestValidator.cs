using FastEndpoints;
using FluentValidation;

namespace Starlights.Modules.Elements.Endpoints.Content.Feats.GetById;

public sealed class GetFeatByIdRequestValidator : Validator<GetFeatByIdRequest>
{
    public GetFeatByIdRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}
